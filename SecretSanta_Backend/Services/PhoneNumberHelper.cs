namespace SecretSanta_Backend.Services
{
    public static class PhoneNumberHelper
    {
        public static string PhoneDbFormat(this string number)
        {
            number = number.Replace("+7", "");
            number = number.Replace("(", "");
            number = number.Replace(")", "");
            number = number.Replace("-", "");
            number = number.Replace(" ", "");
            if (number.Length > 10)
                number = number.Remove(0,1);

            return number;
        }

        public static string PhoneViewFormat(this string number)
        {
            var result = string.Format("{0:+7 (###) ###-##-##}", long.Parse(number));
            return result;
        }
    }
}
