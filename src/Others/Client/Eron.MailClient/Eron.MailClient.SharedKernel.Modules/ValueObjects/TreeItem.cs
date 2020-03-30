using System.Collections.Generic;

namespace Eron.MailClient.SharedKernel.Modules.ValueObjects
{
    public class TreeItem<T>
    {
        public T Item { get; set; }

        public IEnumerable<TreeItem<T>> Children { get; set; }
    }
}