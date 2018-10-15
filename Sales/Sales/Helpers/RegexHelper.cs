namespace Sales.Helpers
{
    using System;
    using System.Net.Mail;

    public static class RegexHelper
    {
        public static bool IsValidEmailAddress(string emailaddress)
        {
            try
            {
                //aqui utilizo la clase emailAddress sin necesidad de untilizar una expresion regular:
                var email = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }

}
