﻿using READER_0._1.Model.Settings.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Word.Settings
{
    [Serializable]
    public class MainSearchString : SearchString
    {
        public bool ZeroingStrings { get; set; }        

    }
}
