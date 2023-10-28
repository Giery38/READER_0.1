using READER_0._1.Model.Settings.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Word.Settings
{
    [Serializable]
    public class DynamicSearchWord : SearchWord
    {
        private SearchWord baseSearchWord;
        public SearchWord BaseSearchWord
        {
            get
            {
                return baseSearchWord;
            }
            set
            {
                if (value != this)
                {
                    baseSearchWord = value;
                }  
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }
        private int maxDistance;
        public int MaxDistance
        {
            get
            {
                return maxDistance;
            }
            set
            {
                maxDistance = value;
            }
        }
        private PositionWord positionWord;
        public PositionWord PositionWord
        {
            get
            {
                return positionWord;
            }
            set
            {
                positionWord = value;
            }
        }
        
    }
   public enum PositionWord
   {
        Left,
        Right
   }
}
