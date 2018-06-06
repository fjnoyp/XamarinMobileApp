using Cards.Core.FileReaders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cards.Core
{
    /// <summary>
    /// Interface for a card provider 
    /// </summary>
    public interface ICardProvider
    {
        Task<Card> getCardAsync(); 
    }
}
