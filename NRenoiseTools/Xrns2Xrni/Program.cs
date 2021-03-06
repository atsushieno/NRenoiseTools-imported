// Copyright 2008 Alexandre Mutel
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Windows.Forms;

namespace NRenoiseTools.Xrns2XrniApp
{
    /// <summary>
    /// Extract all instruments in a Renoise Song to xrni files.
    /// </summary>
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Xrns2XrniForm());
            }
            else
            {
                Xrns2Xrni xrns2Xrni = new Xrns2Xrni();
                xrns2Xrni.Extract(args);
            }
        }
    }
}
