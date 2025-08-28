using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class UserFile
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; } = 0;

    }
}
