using ProductCodeManagement.Services.Abstract;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ProductCodeManagement.Services.Concerete
{
    public class ProductCodeService : IProductCodeService
    {
        // Karakter seti daha komplike ve rastgeleliği artması adına karışık ve sayıları arttırılarak oluşturulmuştur.

        public static readonly char[] characterFullSet = new char[] { 'A','C','D','E','F','G','H','K','L','M','N','P','R','T','X','Y','Z','2','3','4','5','7','9',
                                           'F', 'G', 'H', 'K', 'L', 'M', 'N', 'P', 'R', 'P', 'R', 'T', 'X', 'Y', 'Z', '2','P','R','T' };
        public static readonly char[] oddAsciiSet = new char[] { 'A', 'D', 'E', 'F', 'G', 'H', 'K', 'L', 'N', 'P', 'R', 'T', 'X', '3', '5', '7', '9' };
        public static readonly char[] evenAsciiSet = new char[] { 'C', '2', '4' };
        public static readonly string firstGroup = "KLFHM34DEA5";
        public static readonly string secondGroup = "N7CGY9PR";
        public static readonly string thirdGroup = "MTLXZ2";
        public static readonly string failMessage = "Code is not valid.";
        public static readonly string successMessage = "Code is valid";
        public static readonly string pattern = @"^[ACDEFGHKLMNPRTXYZ234579]+$";

        public List<string> GenerateCode(int count)
        {
            List<string> codeList = new List<string>();
            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                StringBuilder sb = new StringBuilder();



                char firstChar = CreateFirstChar(random, sb);
                int firstCharIndex = Array.IndexOf(characterFullSet, firstChar);

                char secondChar = CreateSecondChar(random, sb, firstChar);
                int secondCharIndex = Array.IndexOf(characterFullSet, secondChar);


                char thirdChar = CreateThirdChar(random, sb, firstChar, secondChar);
                int thirdCharIndex = Array.IndexOf(characterFullSet, thirdChar);


                char fourthChar = CreateFourthChar(random, sb, firstChar, secondChar, thirdChar);


                int sumIndex = firstCharIndex + secondCharIndex + thirdCharIndex;
                char fifthChar = CrateFifthChar(sb, sumIndex, fourthChar);
                int fifthCharIndex = Array.IndexOf(characterFullSet.ToArray(), fifthChar);
                sumIndex += fifthCharIndex;

                char sixthChar = CreateSixthChar(random, sb, firstChar, thirdChar, fifthChar);
                int sixthCharIndex = Array.IndexOf(characterFullSet.ToArray(), sixthChar);
                sumIndex += sixthCharIndex;


                char seventhChar = CreateSeventhChar(random, sb, sumIndex);
                int seventhCharIndex = Array.IndexOf(characterFullSet.ToArray(), seventhChar);
                sumIndex += seventhCharIndex;

                CreateEighthChar(sb, firstCharIndex, secondCharIndex, thirdCharIndex, fifthCharIndex, seventhCharIndex);

                codeList.Add(sb.ToString());


            }



            return codeList.Distinct().ToList();
        }


        
        public string CheckCode(string code)
        {


            if (code.Length != 8)
            {
                return failMessage;
            }

            if (!Regex.IsMatch(code, pattern))
            {
                return failMessage;
            }

            var codeCharArray = code.ToCharArray();
            int firstIndex = Array.IndexOf(characterFullSet, codeCharArray[0]);
            int secondIndex = Array.IndexOf(characterFullSet, codeCharArray[1]);
            int thirdIndex = Array.IndexOf(characterFullSet, codeCharArray[2]);
            int fifthIndex = Array.IndexOf(characterFullSet, codeCharArray[4]);
            int sixthIndex = Array.IndexOf(characterFullSet, codeCharArray[5]);
            int seventhIndex = Array.IndexOf(characterFullSet, codeCharArray[6]);
            int sumIndex = firstIndex + secondIndex + thirdIndex;
            
            bool isFifthChar = CheckFifthChar(codeCharArray[3],sumIndex, codeCharArray[4]);

            if (!isFifthChar)
            {
                return failMessage;
            }

            sumIndex = sumIndex + fifthIndex + sixthIndex;
            bool isSeventhChar = CheckSeventhChar(sumIndex, codeCharArray[6]);

            if (!isSeventhChar)
            {
                return failMessage;
            }

            bool isEighthChar = CheckEighthChar(firstIndex, secondIndex, thirdIndex, fifthIndex, seventhIndex, codeCharArray[7]);

            if (!isEighthChar)
            {
                return failMessage;
            }

            return successMessage;
        }



        private bool CheckEighthChar(int firstCharIndex, int secondCharIndex, int thirdCharIndex, int fifthCharIndex, int seventhCharIndex,char codeEighthChar)
        {
            char eighthChar = characterFullSet[(firstCharIndex * 2 + secondCharIndex * 3 + thirdCharIndex * 141 + fifthCharIndex * 22 + seventhCharIndex * 3) % 42];

            return eighthChar == codeEighthChar;
        }

        private bool CheckSeventhChar(int sumIndex,char codeSeventhChar)
        {

            if (sumIndex % 2 == 0)
            {
                return evenAsciiSet.Contains(codeSeventhChar);
            }

             return oddAsciiSet.Contains(codeSeventhChar);
            
        }

        private bool CheckFifthChar(char fourthChar,int sumIndex,char codeFifthChar)
        {
            char fifthChar = ' ';
            if (firstGroup.Contains(fourthChar))
            {
                int fourthCharIndex = Array.IndexOf(firstGroup.ToArray(), fourthChar);
                fifthChar = characterFullSet[(sumIndex + fourthCharIndex) % 42];



            }
            else if (secondGroup.Contains(fourthChar))
            {
                int fourthCharIndex = Array.IndexOf(secondGroup.ToArray(), fourthChar);
                fifthChar = characterFullSet[(sumIndex + fourthCharIndex) % 42];
            }
            else
            {
                int fourthCharIndex = Array.IndexOf(thirdGroup.ToArray(), fourthChar);
                fifthChar = characterFullSet[(sumIndex + fourthCharIndex) % 42];
            }

            return fifthChar == codeFifthChar;
        }


        /// <summary>
        /// CharacterFullSet setinden bir elemanı fonksiyona göre seçer.
        /// </summary>
        /// <param name="characterFullSet"></param>
        /// <param name="sb"></param>
        /// <param name="firstCharIndex"></param>
        /// <param name="secondCharIndex"></param>
        /// <param name="thirdCharIndex"></param>
        /// <param name="fifthCharIndex"></param>
        /// <param name="seventhCharIndex"></param>

        private void CreateEighthChar(StringBuilder sb, int firstCharIndex, int secondCharIndex, int thirdCharIndex, int fifthCharIndex, int seventhCharIndex)
        {
            char eighthChar = characterFullSet[(firstCharIndex * 2 + secondCharIndex * 3 + thirdCharIndex * 141 + fifthCharIndex * 22 + seventhCharIndex * 3) % 42];
            sb.Append(eighthChar);
        }

        /// <summary>
        /// Indexlerin toplamının tek ya da çifte bağlı olarak  ASCII tablosundaki tek ya da çift değerlere bağlı olarak oluşturulan evenAscii ve oddAscii kümelerinden rastgele eleman seçer.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="oddAsciiSet"></param>
        /// <param name="evenAsciiSet"></param>
        /// <param name="characterFullSet"></param>
        /// <param name="sb"></param>
        /// <param name="sumIndex"></param>
        /// <returns></returns>
        private char CreateSeventhChar(Random random, StringBuilder sb, int sumIndex)
        {
            char seventhChar = ' ';

            if (sumIndex % 2 == 0)
            {
                seventhChar = evenAsciiSet[random.Next(3)];
            }
            else
            {
                seventhChar = oddAsciiSet[random.Next(17)];
            }

            sb.Append(seventhChar);
            return seventhChar;
        }

        /// <summary>
        /// CharacterFullSet setinden rastgele bir elemanı verilen yapıya göre seçer.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="characterFullSet"></param>
        /// <param name="characterFullSet"></param>
        /// <param name="sb"></param>
        /// <param name="firstChar"></param>
        /// <param name="thirdChar"></param>
        /// <param name="sumIndex"></param>
        /// <param name="fifthChar"></param>
        /// <returns></returns>
        private char CreateSixthChar(Random random, StringBuilder sb, char firstChar, char thirdChar, char fifthChar)
        {
            char sixthChar = characterFullSet[random.NextInt64(Math.Abs(((int)firstChar * (int)thirdChar * (int)fifthChar * 786431 + 45) % 42))];


            sb.Append(sixthChar);
            return sixthChar;
        }

        /// <summary>
        /// Dördüncü charın değeri hangi gruba aitse o gruptaki index değeri ile beşinci char hesaplanır.
        /// </summary>
        /// <param name="characterFullSet"></param>
        /// <param name="characterFullSet"></param>
        /// <param name="firstGroup"></param>
        /// <param name="secondGroup"></param>
        /// <param name="thirdGroup"></param>
        /// <param name="sb"></param>
        /// <param name="firstCharIndex"></param>
        /// <param name="secondCharIndex"></param>
        /// <param name="thirdCharIndex"></param>
        /// <param name="fourthChar"></param>
        /// <param name="sumIndex"></param>
        /// <param name="fifthChar"></param>
        /// <param name="fifthCharIndex"></param>
        private char CrateFifthChar(StringBuilder sb, int sumIndex, char fourthChar)
        {
            char fifthChar = ' ';
            if (firstGroup.Contains(fourthChar))
            {
                int fourthCharIndex = Array.IndexOf(firstGroup.ToArray(), fourthChar);
                fifthChar = characterFullSet[(sumIndex + fourthCharIndex) % 42];



            }
            else if (secondGroup.Contains(fourthChar))
            {
                int fourthCharIndex = Array.IndexOf(secondGroup.ToArray(), fourthChar);
                fifthChar = characterFullSet[(sumIndex + fourthCharIndex) % 42];
            }
            else
            {
                int fourthCharIndex = Array.IndexOf(thirdGroup.ToArray(), fourthChar);
                fifthChar = characterFullSet[(sumIndex + fourthCharIndex) % 42];
            }


            sb.Append(fifthChar);
            return fifthChar;
        }

        /// <summary>
        /// CharacterFullSet setinden rastgele bir elemanı random fonksiyonundaki algoritmaya uygun şekilde seçer.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="characterFullSet"></param>
        /// <param name="stringCharSet"></param>
        /// <param name="sb"></param>
        /// <param name="firstChar"></param>
        /// <param name="secondChar"></param>
        /// <param name="thirdChar"></param>
        /// <returns></returns>
        private char CreateFourthChar(Random random, StringBuilder sb, char firstChar, char secondChar, char thirdChar)
        {
            char generatedChar = characterFullSet[random.NextInt64(Math.Abs(((int)firstChar * (int)secondChar * (int)thirdChar * 66047 + 14) % 42))];

            char fourthChar = ' ';
            int minDiff = int.MaxValue;
            foreach (char c in characterFullSet)
            {
                byte cByte = Convert.ToByte(c);
                int diff = Math.Abs(cByte - generatedChar);
                if (diff < minDiff)
                {
                    fourthChar = c;
                    minDiff = diff;
                }
            }
            sb.Append(fourthChar);
            return fourthChar;
        }

        /// <summary>
        /// CharacterFullSet setinden rastgele bir elemanı verilen yapıya göre seçer.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="characterFullSet"></param>
        /// <param name="stringCharSet"></param>
        /// <param name="sb"></param>
        /// <param name="firstChar"></param>
        /// <param name="secondChar"></param>
        /// <param name="thirdChar"></param>
        /// <param name="thirdCharIndex"></param>
        private char CreateThirdChar(Random random, StringBuilder sb, char firstChar, char secondChar)
        {
            char thirdChar = characterFullSet[random.NextInt64(Math.Abs((int)firstChar * (int)secondChar * 786431 + 11) % 42)];
            sb.Append(thirdChar);
            return thirdChar;
        }

        /// <summary>
        /// CharacterFullSet setinden rastgele bir elemanı random fonksiyonundaki algoritmaya uygun şekilde seçer.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="characterFullSet"></param>
        /// <param name="stringCharSet"></param>
        /// <param name="sb"></param>
        /// <param name="firstChar"></param>
        /// <param name="secondChar"></param>
        /// <param name="secondCharIndex"></param>
        private char CreateSecondChar(Random random, StringBuilder sb, char firstChar)
        {
            char secondChar = characterFullSet[random.NextInt64(Math.Abs(((int)firstChar * 524287 + 113) % 42))];
            sb.Append(secondChar);
            return secondChar;
        }

        /// <summary>
        /// CharacterFullSet setinden rastgele bir elemanı random fonksiyonunu çıktısına göre seçer.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="characterFullSet"></param>
        /// <param name="stringCharSet"></param>
        /// <param name="sb"></param>
        /// <param name="firstChar"></param>
        /// <param name="firstCharIndex"></param>
        private char CreateFirstChar(Random random, StringBuilder sb)
        {
            char firstChar = characterFullSet[random.Next(characterFullSet.Length)];
            sb.Append(firstChar);
            return firstChar;
        }
    }
}

