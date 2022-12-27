using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class BaseEntity<TKey>
    {
        public TKey Id { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }
    }
}