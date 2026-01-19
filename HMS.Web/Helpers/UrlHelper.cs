namespace HMS.Web.Helpers
{
    public static class UrlHelper
    {
        public static string GetDoctorInitials(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "N/A";

            var parts = name.Split(' ');
            return string.Concat(parts.Select(p => p[0])).ToUpper();
        }

        public static string GetStatusColor(string status)
        {
            return status?.ToLower() switch
            {
                "scheduled" => "primary",
                "pending" => "warning",
                "completed" => "success",
                "cancelled" => "danger",
                _ => "secondary"
            };
        }

        public static string FormatPhoneNumber(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return string.Empty;

            phone = new string(phone.Where(char.IsDigit).ToArray());

            if (phone.Length == 10)
                return $"({phone.Substring(0, 3)}) {phone.Substring(3, 3)}-{phone.Substring(6)}";

            return phone;
        }

        public static string FormatCurrency(decimal amount)
        {
            return amount.ToString("C");
        }
    }
}
