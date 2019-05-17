using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElektronikSoyAğacı
{
    public class Birey
    {
       public String Ad;
       public String Soyad;
       public int dogumTarihi;
       public int nesil;
       public Boolean cinsiyet;
       public int numara;
    
    public String kadinErkek()
        {
            return cinsiyet ? "Erkek" : "Kadın";
        }
    }
}
