namespace Eron.MailClient.SharedKernel.Modules.ValueObjects
{
    public class SelectListObject
    {
        public SelectListObject(object id, string displayName, bool isSelected = false)
        {
            this.DisplayName = displayName;
            this.Id = id.ToString();
            this.IsSelected = isSelected;
        }

        public string Id { get; set; }

        public string DisplayName { get; set; }

        public bool IsSelected { get; set; }
    }
}