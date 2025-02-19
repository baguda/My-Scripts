using System.Linq;
using UnityEngine;

namespace CogSim
{

    public class MapObjectIdentificationHandler
    {
        private int _currentIndex;
        public MapObjectIdentificationHandler()
        {
            this._currentIndex = 0;
        }
        public string GenerateIndex(string NameTag)
        {
            string result = NameTag + _currentIndex;
            _currentIndex++;
            return result;
        }
    }
}
