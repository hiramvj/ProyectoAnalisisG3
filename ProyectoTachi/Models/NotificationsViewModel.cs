namespace ProyectoTachi.Models
{
    public class NotificationsViewModel
    {
        public List<NotificationItemViewModel> Items { get; set; } = new();
        public int Total => Items.Count;
    }

    public class NotificationItemViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public string Icon { get; set; } = "fa-solid fa-circle-info";
        public string ColorClass { get; set; } = "text-primary";
        public string Controller { get; set; } = "Home";
        public string Action { get; set; } = "Index";
    }
}
