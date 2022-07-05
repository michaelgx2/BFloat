using System;
using System.Text;

public struct BFloat
{
    /// <summary>
    /// 最大精确位数，Windows下标准为14，其它平台可按需修改
    /// </summary>
    public static int MaxDigitScale = 14;

    private static char[] _letters = new[]
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
        'w', 'x', 'y', 'z'
    };
    private double _num;
    private double _scale;

    public BFloat(double num)
    {
        _num = num;
        _scale = 0;
        CutNumber();
    }

    public BFloat(double num, int scale)
    {
        _num = num;
        _scale = scale;
        CutNumber();
    }

    private string GetIntPart(double num)
    {
        return Math.Truncate(num).ToString("0");
    }

    /// <summary>
    /// 调整数字位数到标准输出
    /// </summary>
    private void CutNumber()
    {
        string intPart = GetIntPart(_num);
        if (intPart == "0" && _scale > 0)
        {
            if (_num != 0.0)
            {
                while (GetIntPart(_num) == "0")
                {
                    _scale -= 1;
                    _num *= 10;
                }
            }
        }
        else
        {
            int numScale = intPart.Length - (_num < 0 ? 1 : 0); //获取整数部分长度，负数需要额外-1
            _scale += numScale - 1;
            _num /= Math.Pow(10, numScale - 1);
        }
    }

    /// <summary>
    /// 强制修改数字位数
    /// </summary>
    /// <param name="newScale">新的数字位数</param>
    private void ChangeScale(double newScale)
    {
        double diff = newScale - _scale;
        if (diff > MaxDigitScale)
        {
            _num = 0;
            _scale = newScale;
        }
        else if (diff < -4)
        {
            throw new InvalidOperationException("不允许的移位方向。");
        }
        else
        {
            _num /= Math.Pow(10, diff);
            _scale = newScale;
        }
    }

    #region 隐式转换
    public static implicit operator BFloat(int x)
    {
        return new BFloat(x);
    }
    public static implicit operator BFloat(float x)
    {
        return new BFloat(x);
    }
    public static implicit operator BFloat(double x)
    {
        return new BFloat(x);
    }
    public static implicit operator BFloat(long x)
    {
        return new BFloat(x);
    }
    public static implicit operator BFloat(byte x)
    {
        return new BFloat(x);
    }
    public static implicit operator BFloat(short x)
    {
        return new BFloat(x);
    }
    public static implicit operator BFloat(sbyte x)
    {
        return new BFloat(x);
    }
    public static implicit operator BFloat(uint x)
    {
        return new BFloat(x);
    }
    public static implicit operator BFloat(ulong x)
    {
        return new BFloat(x);
    }
    public static implicit operator BFloat(ushort x)
    {
        return new BFloat(x);
    }
    public static implicit operator BFloat(char x)
    {
        return new BFloat(x);
    }

    #endregion

    #region 运算符重载

    public static BFloat operator +(BFloat left, BFloat right)
    {
        BFloat ret = left;
        //修正位数
        if (left._scale > right._scale)
        {
            right.ChangeScale(left._scale);
        }
        else if (left._scale < right._scale)
        {
            left.ChangeScale(right._scale);
        }

        ret._scale = left._scale;
        ret._num = left._num + right._num;
        ret.CutNumber();
        return ret;
    }

    public static BFloat operator -(BFloat left, BFloat right)
    {
        BFloat ret = left;
        //修正位数
        if (left._scale > right._scale)
        {
            right.ChangeScale(left._scale);
        }
        else if (left._scale < right._scale)
        {
            left.ChangeScale(right._scale);
        }

        ret._scale = left._scale;
        ret._num = left._num - right._num;
        ret.CutNumber();
        return ret;
    }

    public static BFloat operator *(BFloat left, BFloat right)
    {
        BFloat ret = left;
        ret._num = left._num * right._num;
        ret._scale = left._scale + right._scale;
        ret.CutNumber();
        return ret;
    }

    public static BFloat operator /(BFloat left, BFloat right)
    {
        BFloat ret = left;
        if (left._scale < right._scale)
        {
            left.ChangeScale(right._scale);
        }
        ret._num = left._num / right._num;
        ret._scale = left._scale - right._scale;
        ret.CutNumber();
        return ret;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        BFloat o = (BFloat)obj;
        return _num == o._scale && _scale == o._scale;
    }

    public static bool operator >(BFloat left, BFloat right)
    {
        //修正位数
        if (left._scale > right._scale)
        {
            right.ChangeScale(left._scale);
        }
        else if (left._scale < right._scale)
        {
            left.ChangeScale(right._scale);
        }

        return left._num > right._num;
    }

    public static bool operator <(BFloat left, BFloat right)
    {
        //修正位数
        if (left._scale > right._scale)
        {
            right.ChangeScale(left._scale);
        }
        else if (left._scale < right._scale)
        {
            left.ChangeScale(right._scale);
        }

        return left._num < right._num;
    }

    public static bool operator ==(BFloat left, BFloat right)
    {
        //修正位数
        if (left._scale > right._scale)
        {
            right.ChangeScale(left._scale);
        }
        else if (left._scale < right._scale)
        {
            left.ChangeScale(right._scale);
        }

        return left._num == right._num;
    }

    public static bool operator !=(BFloat left, BFloat right)
    {
        //修正位数
        if (left._scale > right._scale)
        {
            right.ChangeScale(left._scale);
        }
        else if (left._scale < right._scale)
        {
            left.ChangeScale(right._scale);
        }

        return left._num == right._num;
    }

    public static BFloat Pow(BFloat x, double y)
    {
        BFloat ret = x;
        ret._scale = x._scale * y;
        ret._num = Math.Pow(x._num, y);
        ret.CutNumber();
        return ret;
    }
    #endregion

    public override string ToString()
    {
        return ToString(BFloatStringFormat.UseLetters);
    }

    public string ToString(BFloatStringFormat format)
    {
        string ret = "";
        if (format == BFloatStringFormat.Scientific)
        {
            if (_num == 0)
            {
                ret = "0";
            }
            else if (_scale <= 3)
            {
                ret = (_num * Math.Pow(10, _scale)).ToString("0.###");
            }
            else if (_scale <= 6)
            {
                ret = (_num * Math.Pow(10, _scale)).ToString("0");
            }
            else
            {
                ret = $"{_num:0.###}e{_scale:0}";
            }
        }
        else if (format == BFloatStringFormat.UseLetters)
        {
            if (_num == 0)
            {
                ret = "0";
            }
            else if (_scale < 3)
            {
                ret = (_num * Math.Pow(10, _scale)).ToString("0.###");
            }
            else
            {
                //先求出位数对3的余数，确定需要进行移位的次数
                int needChange = (int)(Math.Truncate(_scale) % 3);
                //然后进行小数点向右的余数次位移，使得位数变为3的倍数
                if (needChange != 0)
                {
                    ChangeScale(_scale - needChange);
                }
                //根据当前的实数和位数进行字符化
                string unit = "";
                if (_scale <= 3)
                {
                    unit = "k";
                }
                else if (_scale <= 6)
                {
                    unit = "m";
                }
                else if (_scale <= 9)
                {
                    unit = "b";
                }
                else if (_scale <= 12)
                {
                    unit = "t";
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    var ss = (int)(((_scale - 13) / 3) % 26);//26字母余数
                    sb.Insert(0, _letters[ss]);//先插入最后一位字母
                    //循环处理轮数，反复求余
                    var sd = Math.Truncate(((_scale - 13) / 3) / 26 + 1);//26字母轮数
                    while (true)
                    {
                        if (sd <= 26)
                        {
                            sb.Insert(0, _letters[(int)sd - 1]);
                            break;
                        }
                        else
                        {
                            ss = (int)((sd) % 26);//26字母余数
                            sb.Insert(0, _letters[ss]);//先插入最后一位字母
                            sd = Math.Truncate(sd / 26);//26字母轮数
                        }
                    }

                    unit = sb.ToString();
                }
                ret = _num.ToString("0.###") + unit;
            }

        }
        else if (format == BFloatStringFormat.ChineseAndLetters)
        {
            if (_num == 0)
            {
                ret = "0";
            }
            else if (_scale < 4)
            {
                ret = (_num * Math.Pow(10, _scale)).ToString("0.###");
            }
            else
            {
                if (_scale <= 15)
                {
                    string unit = "";
                    int needChange = (int)(Math.Truncate(_scale) % 4);
                    //然后进行小数点向右的余数次位移，使得位数变为3的倍数
                    if (needChange != 0)
                    {
                        ChangeScale(_scale - needChange);
                    }
                    if (_scale <= 4)
                    {
                        unit = "万";
                    }
                    else if (_scale <= 8)
                    {
                        unit = "亿";
                    }
                    else if (_scale <= 12)
                    {
                        unit = "万亿";
                    }
                    else
                    {
                        unit = "兆";
                    }
                    ret = _num.ToString("0.##") + unit;
                }
                else
                {
                    string unit = "";
                    //先求出位数对3的余数，确定需要进行移位的次数
                    int needChange = (int)(Math.Truncate(_scale) % 3);
                    //然后进行小数点向右的余数次位移，使得位数变为3的倍数
                    if (needChange != 0)
                    {
                        ChangeScale(_scale - needChange);
                    }

                    //根据当前的实数和位数进行字符化
                    StringBuilder sb = new StringBuilder();
                    var ss = (int)(((_scale - 13) / 3) % 26); //26字母余数
                    sb.Insert(0, _letters[ss]); //先插入最后一位字母
                                                //循环处理轮数，反复求余
                    var sd = Math.Truncate(((_scale - 13) / 3) / 26 + 1); //26字母轮数
                    while (true)
                    {
                        if (sd <= 26)
                        {
                            sb.Insert(0, _letters[(int)sd - 1]);
                            break;
                        }
                        else
                        {
                            ss = (int)((sd) % 26); //26字母余数
                            sb.Insert(0, _letters[ss]); //先插入最后一位字母
                            sd = Math.Truncate(sd / 26); //26字母轮数
                        }
                    }

                    unit = sb.ToString();

                    ret = _num.ToString("0.###") + unit;
                }
            }
        }

        return ret;
    }
}

public enum BFloatStringFormat
{
    /// <summary>
    /// 科学计数法
    /// </summary>
    Scientific,
    /// <summary>
    /// 使用字母
    /// </summary>
    UseLetters,
    /// <summary>
    /// 使用中文和字母
    /// </summary>
    ChineseAndLetters
}
