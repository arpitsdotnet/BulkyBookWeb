namespace BulkyBook.Utilities;
public class SD
{
    public class Role
    {
        public const string Customer = "Customer";
        public const string Company = "Company";
        public const string Admin = "Admin";
        public const string Employee = "Employee";
    }
    public class DateFormat
    {
        public const string Date = "dd-MMM-yyyy";
        public const string Time = "hh:mm tt";
    }

    public class OrderStatus
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string InProcess = "Processing";
        public const string Shipped = "Shipped";
        public const string Cancelled = "Cancelled";
        public const string Refunded = "Refunded";
    }

    public class PaymentStatus
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string DelayedPayment = "ApprovedForDelayedPayment";
        public const string Rejected = "Rejected";
        public const string Refunded = "Refunded";

    }
}
