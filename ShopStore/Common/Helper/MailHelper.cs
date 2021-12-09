﻿using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;

namespace ShopStore.Common
{
    public static class MailHelper
    {

        public static string SendMail(string memberName, string mailAddress)
        {
            MimeMessage mail = new MimeMessage();
            mail.From.Add(new MailboxAddress("linjim", "linjim1101@gmail.com"));
            mail.To.Add(new MailboxAddress(memberName, mailAddress));
            mail.Priority = MessagePriority.Normal;
            mail.Subject = $"{memberName} 感謝您註冊 ShopStore";

            //產生隨機認證碼
            Random random = new Random();
            string code = random.Next(0, 9999).ToString("0000");

            //建立 html 郵件格式
            BodyBuilder bodyBuilder = new BodyBuilder
            {
                HtmlBody =
                $@"<h1>{memberName} 感謝您的註冊！</h1>
                   <h2>您的信箱驗證碼是：{code}</h2>"
            };

            //設定郵件內容
            mail.Body = bodyBuilder.ToMessageBody(); //轉成郵件內容格式

            using var client = new SmtpClient();
            client.CheckCertificateRevocation = false; //有開防毒時需設定 false 關閉檢查
            //設定連線 gmail ("smtp Server", Port, SSL加密) 
            client.Connect("smtp.gmail.com", 587, false); // localhost 測試使用加密需先關閉 
            // Note: only needed if the SMTP server requires authentication
            client.Authenticate("linjim1101@gmail.com", "jmjygxjclxuvtlaj");
            client.Send(mail); //發信            
            client.Disconnect(true); //結束連線

            return code;
        }
    }
}
