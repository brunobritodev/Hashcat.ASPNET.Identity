using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace DictionaryAttackForDummies
{
    class Dictionary : IDisposable
    {
        public Dictionary(string path)
        {
            this.Passwords = new StreamReader(File.OpenRead(path));
        }

        public StreamReader Passwords { get; set; }

        public string GetPassword()
        {
            return this.Passwords.ReadLine();
        }
        
        public void Dispose()
        {
            this.Passwords.Close();
            this.Passwords.Dispose();
        }
    }
}
