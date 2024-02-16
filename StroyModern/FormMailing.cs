using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StroyModern
{
    public partial class FormMailing : Form
    {
        private readonly CheckUser _user;
        public FormMailing(CheckUser user)
        {
            InitializeComponent();
            _user = user;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string smtpServer = "smtp.mail.ru";
            int smtpPort = 587;

            string smtpUsername = "waguteru@mail.ru";
            string smtpPassword = "kG6K9KvM5PtpENRLi1Vp";

            using(SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true;

                using(MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(smtpUsername);
                    mailMessage.To.Add("zhukanas04@mail.ru");
                    mailMessage.Subject = txt3.Text;
                    mailMessage.Body = txt2.Text;

                    try
                    {
                        smtpClient.Send(mailMessage);
                       MessageBox.Show("Сообщение отправлено");
                        FormProductsAdmin form1 = new FormProductsAdmin(_user);
                        form1.Show();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Сообщение не отправлено {ex.Message}");
                    }
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormProductsAdmin form1 = new FormProductsAdmin(_user);
            form1.Show();
            this.Close();
        }

        private void FormMailing_Load(object sender, EventArgs e)
        {
            label2.Text = $"{_user.Login}:{_user.Status}";
        }
    }
}
