﻿using System;
using System.Net.Mail;
using System.Net;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data;
using Portal.Areas.Admin.Models;
using System.Text.RegularExpressions;
using PgDbase;
using cms.dbModel.entity;

/// <summary>
/// Сервис отправки писем
/// </summary>
public class Mailer
{

    /// <summary>
    /// Контекст доступа к базе данных
    /// </summary>
    //protected dbRepository _repository { get; private set; }
    //protected SettingsViewModel model = new SettingsViewModel();
    protected string domain = "";

    protected string server = String.Empty;
    protected int port = 25;
    protected bool ssl = false;

    protected string maillist = String.Empty;
    protected string mailfrom = String.Empty;
    protected string mailname = String.Empty;
    protected string password = String.Empty;

    protected string theme = "Обратная связь";
    protected string text = String.Empty;
    protected string styles = String.Empty;

    protected string attechments = String.Empty;
    protected string dublicate = String.Empty;

    public string MailTo
    {
        set { maillist = value; }
        get { return maillist; }
    }

    public string Theme
    {
        set { theme = value; }
        get { return theme; }
    }

    public string Text
    {
        set { text = value; }
        get { return text; }
    }


    public string MailFrom
    {
        set { mailfrom = value; }
        get { return mailfrom; }
    }

    public string MailName
    {
        set { mailname = value; }
        get { return mailname; }
    }

    public string Server
    {
        set { server = value; }
        get { return server; }
    }
    public int Port
    {
        set { port = value; }
        get { return port; }
    }

    public string Password
    {
        set { password = value; }
        get { return password; }
    }

    public bool isSsl
    {
        set { ssl = value; }
        get { return ssl; }
    }

    public string Attachments
    {
        set { attechments = value; }
        get { return attechments; }
    }

    public String Dublicate
    {
        set { dublicate = value; }
        get { return dublicate; }
    }

    public String Domain
    {
        set { domain = value; }
        get { return domain; }
    }

    //Создаем событие, на которое потом подпишемся
    public static event EventHandler<DislyEventArgs> DislyEvent;
    private static void OnDislyEvent(DislyEventArgs eventArgs)
    {
        DislyEvent(null, eventArgs);
    }

    public void MailFromSettings()
    {
        //    try {

        //        _repository = new dbRepository("cmsdbConnection");
        //        SettingsViewModel model = new SettingsViewModel()
        //        {
        //            siteSettings = _repository.getCmsSettings()
        //        };
        //        string MailServer = model.siteSettings.MailServer;
        //        string MailFrom = model.siteSettings.MailFrom;
        //        string MailFromAlias = model.siteSettings.MailFromAlias;
        //        MailFromAlias = (MailFromAlias != String.Empty) ? MailFromAlias : Settings.MailAdresName;
        //        string MailPass = model.siteSettings.MailPass;
        //        string MailTo = model.siteSettings.MailTo;

        //        if (MailFrom == null || MailServer == null || MailPass == null)
        //        {
        //            MailFrom = Settings.MailFrom;
        //            MailServer = Settings.mailServer;
        //            MailPass = Settings.mailPWD;
        //        }


        //        if (mailfrom == String.Empty || server == String.Empty || password == String.Empty)
        //        {
        //            mailfrom = MailFrom;
        //            server = MailServer;
        //            password = MailPass;
        //        }
        //        if (mailname == String.Empty) mailname = MailFromAlias;
        //        if (maillist == String.Empty) maillist = MailTo;


        //        if (mailfrom == null || server == null || password == null)
        //        {
        //            mailname = Settings.MailAdresName;
        //            mailfrom = Settings.MailFrom;
        //            server = Settings.mailServer;
        //            password = Settings.mailPWD;
        //        }

        //}
        //    catch {
        //        mailname = Settings.MailAdresName;
        //        mailfrom = Settings.MailFrom;
        //        server = Settings.mailServer;
        //        password = Settings.mailPWD;
        //    }      
        try
        {
            if (mailfrom == String.Empty || server == String.Empty || password == String.Empty)
            {
                server = Settings.mailServer;
                port = Settings.mailServerPort;
                ssl = Settings.mailServerSSL;
                mailname = Settings.mailAddresName;
                mailfrom = Settings.mailUser;
                server = Settings.mailServer;
                password = Settings.mailPass;
            }
        }
        catch (Exception ex)
        {
            var message = String.Format("Mailer => MailFromSettings");
            OnDislyEvent(new DislyEventArgs(LogLevelEnum.Warning, message, ex));
        }
    }


    public bool SendMail()
    {
        MailFromSettings();
        try
        {
            //Авторизация на SMTP сервере
            SmtpClient Smtp = new SmtpClient(server, port);
            Smtp.EnableSsl = ssl;
            Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            Smtp.UseDefaultCredentials = false;
            Smtp.Credentials = new NetworkCredential(mailfrom, password);

            // Формирование сообщения
            MailMessage _Message = new MailMessage();
            _Message.From = new MailAddress(mailfrom, mailname);

            _Message.Subject = theme;
            _Message.BodyEncoding = System.Text.Encoding.UTF8;
            _Message.IsBodyHtml = true;
            _Message.Body = "<DOCTYPE html><html><head></head><body>" + text + "</body></html>";
            if (Attachments != string.Empty)
                _Message.Attachments.Add(new Attachment(Attachments));

            if (dublicate != String.Empty)
                maillist += ";" + dublicate;

            if (maillist != null)
            {
                string[] MailList = maillist.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                Regex regex = new Regex(@"\b[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}\b");
                foreach (string adress in MailList)
                {
                    MatchCollection normail = regex.Matches(adress);
                    if (normail.Count > 0)
                        _Message.To.Add(new MailAddress(adress));
                }

                Smtp.Send(_Message);
                return true;
            }

            var message = String.Format("Mailer => SendMail for domain {0}: не указаны адесаты", domain);
            OnDislyEvent(new DislyEventArgs(LogLevelEnum.Warning, message, null));

        }
        catch (Exception ex)
        {
            var message = String.Format("Mailer => SendMail for domain {0}", domain);
            OnDislyEvent(new DislyEventArgs(LogLevelEnum.Error, message, ex));
        }

        return false;
    }
}
