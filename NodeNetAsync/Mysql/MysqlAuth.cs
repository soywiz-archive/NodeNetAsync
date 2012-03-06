using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Mysql
{
	class MysqlAuth
	{
		static public byte[] Token(byte[] Password, byte[] ScrambleBuffer)
		{
			if (Password == null || Password.Length == 0) return new byte[0];
			var Stage1 = Sha1(Password);
			var Stage2 = Sha1(Stage1);
			var Stage3 = Sha1(ScrambleBuffer.Concat(Stage2).ToArray());
			return Xor(Stage3, Stage1);
		}

		static private byte[] Sha1(byte[] Data)
		{
			return SHA1.Create().ComputeHash(Data);
		}

		static private byte[] Xor(byte[] Left, byte[] Right)
		{
			var Out = new byte[Left.Length];
			for (int n = 0; n < Out.Length; n++) Out[n] = (byte)(Left[n] ^ Right[n]);
			return Out;
		}
	}
}
