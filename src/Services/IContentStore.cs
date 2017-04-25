using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using conmonapi.Models;

namespace conmonapi.Services
{
    public interface IContentSore
    {
        Task<bool> SetContentAsync(Stream stream, Content content);
        Task<Tuple<byte[], Content>> GetContentAsync(Content content);
        Task<IEnumerable<Content>> GetContentsAsync(int skip, int limit); 
        Task<bool> UpdateContentAsync(Stream stream, Content content);
        Task<bool> DeleteContentAsync(Content content);
    }
}