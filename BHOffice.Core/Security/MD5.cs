﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSC = System.Security.Cryptography;

namespace BHOffice.Core.Security
{
    public class MD5 : Digest
    {
        protected override byte[] ComputeHash(byte[] input)
        {
            using (var md5 = SSC.MD5.Create())
            {
                return md5.ComputeHash(input);
            }
        }
    }
}
