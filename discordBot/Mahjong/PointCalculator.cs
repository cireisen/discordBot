using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static discordBot.Mahjong.MahjongGameControl;

namespace discordBot.Mahjong
{
    internal static class PointCalculator
    {
        
        /// <summary>
        /// 마작점수계산
        /// b * 2^(p+2) * (Z1+Z2+Z3)
        /// Z = t : z = 2, z : z = 1
        /// </summary>
        /// <param name="pan"></param>
        /// <param name="fu"></param>
        /// <param name="isTon"></param>
        /// <param name="winType"></param>
        /// <returns></returns>
        public static int CalcPoint(int pan, int fu, int playerCount, bool isTon, WinType winType)
        {
            int z = isTon ? 2 : 1;

            int multiplizer = isTon ? 2 * playerCount : 2 + playerCount - 1;

            decimal point = (decimal)(fu * Math.Pow(2, pan + 2) * multiplizer);

            point = Math.Ceiling(point / 100) * 100;

            return (int)point;
        }
    }
}
