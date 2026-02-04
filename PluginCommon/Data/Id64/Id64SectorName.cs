namespace com.github.fredjk_gh.PluginCommon.Data.Id64
{
    // Adapted from MattG's SectorName implementation.
    internal class Id64SectorName
    {
        public const int SECTORSIZE = 1280;
        public const int GALAXYSIZE = 128;

        private static readonly string[] cx_raw_fragments;
        private static readonly string[] cx_prefixes;
        private static readonly Dictionary<string, int> cx_prefix_length_overrides;
        private static readonly int cx_prefix_length_default = 35;
        private static readonly int cx_prefix_total_run_length;
        private static readonly Dictionary<string, List<int>> prefix_offsets;
        private static readonly Dictionary<string, List<int>> c1_infix_offsets;
        private static readonly Dictionary<string, int> c1_prefix_infix_override_map;
        private static readonly Dictionary<string, int> c2_prefix_suffix_override_map;
        private static readonly string[] c1_infixes_s1;
        private static readonly string[] c1_infixes_s2;
        private static readonly string[] cx_suffixes_s1;
        private static readonly string[] c1_suffixes_s2;
        private static readonly string[] c2_suffixes_s2;
        private static readonly int c1_infix_s1_total_run_length;
        private static readonly int c1_infix_s2_total_run_length;
        private static readonly Dictionary<string, int> c1_infix_length_overrides;

        static Id64SectorName()
        {
            cx_suffixes_s1 = [  "oe",  "io",  "oea", "oi",  "aa",  "ua", "eia", "ae", "ooe", "oo",  "a",   "ue",  "ai",  "e",  "iae", "oae",
                "ou",  "uae", "i",   "ao",  "au",  "o",  "eae", "u", "aea", "ia",  "ie",  "eou", "aei", "ea", "uia", "oa", "aae", "eau", "ee"];
            c1_suffixes_s2 = [   "b", "scs", "wsy", "c", "d", "vsky", "f", "sms", "dst", "g", "rb", "h", "nts", "ch", "rd", "rld",
                "k", "lls", "ck", "rgh", "l", "rg", "m", "n", 
                // Formerly sequence 4/5...
                "hm", "p", "hn", "rk", "q", "rl", "r", "rm", "s", "cs", "wyg", "rn", "ct", "t", "hs", "rbs", "rp", "tts", "v", "wn", "ms", "w", "rr", "mt",
                "x", "rs", "cy", "y", "rt", "z", "ws", "lch", // "y" is speculation
                "my", "ry", "nks", "nd", "sc", "ng", "sh", "nk","sk", "nn", "ds", "sm", "sp", "ns", "nt", "dy","ss", "st", "rrs", "xt", "nz", "sy", "xy", "rsch",
                "rphs", "sts", "sys", "sty", "th", "tl", "tls", "rds","nch", "rns", "ts", "wls", "rnt", "tt", "rdy", "rst",
                "pps", "tz", "tch", "sks", "ppy", "ff", "sps", "kh","sky", "ph", "lts", "wnst", "rth", "ths", "fs", "pp",
                "ft", "ks", "pr", "ps", "pt", "fy", "rts", "ky","rshch", "mly", "py", "bb", "nds", "wry", "zz", "nns",
                "ld", "lf", "gh", "lks", "sly", "lk", "ll", "rph","ln", "bs", "rsts", "gs", "ls", "vvy", "lt", "rks","qs", "rps", "gy", "wns", "lz", "nth", "phs"];
            c2_suffixes_s2 = new string[cx_suffixes_s1.Length];
            Array.Copy(c1_suffixes_s2, c2_suffixes_s2, cx_suffixes_s1.Length);

            c1_infixes_s1 = ["o", "ai", "a", "oi", "ea", "ie", "u", "e", "ee", "oo", "ue", "i", "oa", "au", "ae", "oe"];
            c1_infixes_s2 = ["ll", "ss", "b", "c", "d", "f", "dg", "g","ng", "h", "j", "k", "l", "m", "n", "mb",
                "p", "q", "gn", "th", "r", "s", "t", "ch","tch", "v", "w", "wh", "ck", "x", "y", "z","ph", "sh", "ct", "wr" ];

            cx_raw_fragments = ["Th", "Eo", "Oo", "Eu", "Tr", "Sly", "Dry", "Ou",
                "Tz", "Phl", "Ae", "Sch", "Hyp", "Syst", "Ai", "Kyl","Phr", "Eae", "Ph", "Fl", "Ao", "Scr", "Shr", "Fly",
                "Pl", "Fr", "Au", "Pry", "Pr", "Hyph", "Py", "Chr","Phyl", "Tyr", "Bl", "Cry", "Gl", "Br", "Gr", "By",
                "Aae", "Myc", "Gyr", "Ly", "Myl", "Lych", "Myn", "Ch","Myr", "Cl", "Rh", "Wh", "Pyr", "Cr", "Syn", "Str",
                "Syr", "Cy", "Wr", "Hy", "My", "Sty", "Sc", "Sph","Spl", "A", "Sh", "B", "C", "D", "Sk", "Io",
                "Dr", "E", "Sl", "F", "Sm", "G", "H", "I","Sp", "J", "Sq", "K", "L", "Pyth", "M", "St",
                "N", "O", "Ny", "Lyr", "P", "Sw", "Thr", "Lys","Q", "R", "S", "T", "Ea", "U", "V", "W",
                "Schr", "X", "Ee", "Y", "Z", "Ei", "Oe",

                "ll", "ss", "b", "c", "d", "f", "dg", "g", "ng", "h", "j", "k", "l", "m", "n",
                "mb", "p", "q", "gn", "th", "r", "s", "t", "ch", "tch", "v", "w", "wh",
                "ck", "x", "y", "z", "ph", "sh", "ct", "wr", "o", "ai", "a", "oi", "ea",
                "ie", "u", "e", "ee", "oo", "ue", "i", "oa", "au", "ae", "oe", "scs",
                "wsy", "vsky", "sms", "dst", "rb", "nts", "rd", "rld", "lls", "rgh",
                "rg", "hm", "hn", "rk", "rl", "rm", "cs", "wyg", "rn", "hs", "rbs", "rp",
                "tts", "wn", "ms", "rr", "mt", "rs", "cy", "rt", "ws", "lch", "my", "ry",
                "nks", "nd", "sc", "nk", "sk", "nn", "ds", "sm", "sp", "ns", "nt", "dy",
                "st", "rrs", "xt", "nz", "sy", "xy", "rsch", "rphs", "sts", "sys", "sty",
                "tl", "tls", "rds", "nch", "rns", "ts", "wls", "rnt", "tt", "rdy", "rst",
                "pps", "tz", "sks", "ppy", "ff", "sps", "kh", "sky", "lts", "wnst", "rth",
                "ths", "fs", "pp", "ft", "ks", "pr", "ps", "pt", "fy", "rts", "ky",
                "rshch", "mly", "py", "bb", "nds", "wry", "zz", "nns", "ld", "lf",
                "gh", "lks", "sly", "lk", "rph", "ln", "bs", "rsts", "gs", "ls", "vvy",
                "lt", "rks", "qs", "rps", "gy", "wns", "lz", "nth", "phs", "io", "oea",
                "aa", "ua", "eia", "ooe", "iae", "oae", "ou", "uae", "ao", "eae", "aea",
                "ia", "eou", "aei", "uia", "aae", "eau" ];
            cx_prefixes = new string[111];
            Array.Copy(cx_raw_fragments, cx_prefixes, 111);

            cx_prefix_length_overrides = new()
            {
                {"Eu", 31},{"Sly",  4},{ "Tz",  1},{"Phl", 13},
                { "Ae", 12},{"Hyp", 25},{"Kyl", 30},{"Phr", 10},
                {"Eae",  4},{ "Ao",  5},{"Scr", 24},{"Shr", 11},
                {"Fly", 20},{"Pry",  3},{"Hyph", 14},{ "Py", 12},
                {"Phyl",  8},{"Tyr", 25},{"Cry",  5},{"Aae",  5},
                {"Myc",  2},{"Gyr", 10},{"Myl", 12},{"Lych",  3},
                {"Myn", 10},{"Myr",  4},{ "Rh", 15},{ "Wr", 31},
                {"Sty",  4},{"Spl", 16},{ "Sk", 27},{ "Sq",  7},
                {"Pyth",  1},{"Lyr", 10},{ "Sw", 24},{"Thr", 32},
                {"Lys", 10},{"Schr",  3},{  "Z", 34}
            };

            c1_prefix_infix_override_map = new Dictionary<string, int>
            {
                {"Eo", 2},{"Oo",  2},{"Eu",  2},{"Ou", 2},
                {"Ae", 2},{"Ai",  2},{"Eae", 2},{"Ao", 2},
                {"Au", 2},{"Aae", 2},{"A",   2},{"Io", 2},
                {"E",  2},{"I",   2},{"O",   2},{"Ea", 2},
                {"U",  2},{"Ee",  2},{"Ei",  2},{"Oe", 2}
            };
            c2_prefix_suffix_override_map = new Dictionary<string, int>
            {
                {  "Eo",  2},{"Oo", 2},{"Eu", 2},
                {"Ou",  2},{"Ae", 2},{"Ai", 2},
                {"Eae", 2},{"Ao", 2},{"Au", 2},
                {"Aae", 2}
            };


            cx_prefix_total_run_length = 0;
            for (int i = 0; i < cx_prefixes.Length; i++)
            {
                cx_prefix_total_run_length += cx_prefix_length_overrides.GetValueOrDefault(cx_prefixes[i], cx_prefix_length_default);
            }

            prefix_offsets = [];
            int cnt = 0;
            foreach (string s in cx_prefixes)
            {
                int plen = GetPrefixRunLength(s);
                prefix_offsets.Add(s, [cnt, plen]);
                cnt += plen;
            }


            c1_infix_length_overrides = new()
            {
                // Sequence 1
                {"oi",  88},{"ue", 147},{"oa",  57},
                {"au", 119},{"ae",  12},{"oe",  39},
                // Sequence 2
                {"dg",  31},{"tch",  20},{"wr",  31},
            };

            c1_infix_s1_total_run_length = 0;
            foreach (string s in c1_infixes_s1)
            {
                c1_infix_s1_total_run_length += c1_infix_length_overrides.GetValueOrDefault(s, c1_suffixes_s2.Length);
            }
            c1_infix_s2_total_run_length = 0;
            foreach (string s in c1_infixes_s2)
            {
                c1_infix_s2_total_run_length += c1_infix_length_overrides.GetValueOrDefault(s, cx_suffixes_s1.Length);
            }


            c1_infix_offsets = [];
            cnt = 0;
            foreach (string s in c1_infixes_s1)
            {
                int ilen = C1GetInfixRunLength(s);
                c1_infix_offsets.Add(s, [cnt, ilen]);
                cnt += ilen;
            }
            cnt = 0;
            foreach (string s in c1_infixes_s2)
            {
                int ilen = C1GetInfixRunLength(s);
                c1_infix_offsets.Add(s, [cnt, ilen]);
                cnt += ilen;
            }

        }

        #region Initialization helpers
        //_get_prefix_run_length
        private static int GetPrefixRunLength(string frag)
        {
            return cx_prefix_length_overrides.GetValueOrDefault(frag, cx_prefix_length_default);
        }

        private static int C1GetInfixRunLength(string s)
        {
            if (c1_infix_length_overrides.TryGetValue(s, out int ret))
            { }
            else if (Array.IndexOf(c1_infixes_s1, s) > -1)
            {
                ret = c1_suffixes_s2.Length;
            }
            else
            {
                ret = cx_suffixes_s1.Length;
            }
            return ret;

        }

        #endregion

        public static string GetSectorName(Id64Details system)
        {
            uint offset = GetOffset(system.SectorCoords);
            if (IsTypeC1(offset))
            {
                int cur_offset = (int)(offset % cx_prefix_total_run_length);
                int prefix_cnt = (int)((offset - cur_offset) / cx_prefix_total_run_length);

                string prefix = GetEntryFromOffset(cur_offset, [.. prefix_offsets.Keys], prefix_offsets);
                cur_offset -= prefix_offsets[prefix][0];

                string[] infix1s = C1GetInfixes([prefix]);
                int infix1_total_len = C1GetInfixTotalRunLength(infix1s[0]);
                //infix1_cnt, cur_offset = divmod(prefix_cnt * _get_prefix_run_length(prefix) + cur_offset, infix1_total_len)
                int tmp = (prefix_cnt * GetPrefixRunLength(prefix)) + cur_offset;
                cur_offset = tmp % infix1_total_len;
                int infix1_cnt = (tmp - cur_offset) / infix1_total_len;
                string infix1 = GetEntryFromOffset(cur_offset, [.. infix1s], c1_infix_offsets);
                cur_offset -= c1_infix_offsets[infix1][0];
                int infix1_run_len = C1GetInfixRunLength(infix1);

                string[] frags = [prefix, infix1];
                string[] sufs = GetSuffixes(frags, true);
                int next_idx = (infix1_run_len * infix1_cnt) + cur_offset;

                if (next_idx >= sufs.Length)
                {
                    string[] infix2s = C1GetInfixes(frags);
                    int infix2_total_len = C1GetInfixTotalRunLength(infix2s[0]);
                    //infix2_cnt, cur_offset = divmod(infix1_cnt * _c1_get_infix_run_length(infix1) + cur_offset, infix2_total_len)
                    tmp = (infix1_cnt * C1GetInfixRunLength(infix1)) + cur_offset;
                    cur_offset = tmp % infix2_total_len;
                    int infix2_cnt = (tmp - cur_offset) / infix2_total_len;
                    string infix2 = GetEntryFromOffset(cur_offset, [.. infix2s], c1_infix_offsets);
                    cur_offset -= c1_infix_offsets[infix2][0];
                    int infix2_run_len = C1GetInfixRunLength(infix2);
                    string[] tmpfrags = [prefix, infix1, infix2];
                    sufs = GetSuffixes(tmpfrags, true);
                    next_idx = (infix2_run_len * infix2_cnt) + cur_offset;
                    frags = tmpfrags;
                }

                string[] ret = new string[frags.Length + 1];
                Array.Copy(frags, ret, frags.Length);

                //hack because this breaks for some sectors in original python
                // e.g. 0,0,85
                if (sufs.Length <= next_idx)
                {
                    ret = new string[1];
                    ret[0] = "UNKNOWN";
                }
                else
                {
                    ret[frags.Length] = sufs[next_idx];
                }
                return FormatSectorName(ret);
            }
            else // type = c2
            {
                Deinterleave(offset, 32, out ulong tidx0, out ulong tidx1);
                int idx0 = (int)tidx0;
                int idx1 = (int)tidx1;
                string p0 = GetEntryFromOffset(idx0, [.. prefix_offsets.Keys], prefix_offsets);
                string p1 = GetEntryFromOffset(idx1, [.. prefix_offsets.Keys], prefix_offsets);
                string s0 = GetSuffixes([p0])[idx0 - prefix_offsets[p0][0]];
                string s1 = GetSuffixes([p1])[idx1 - prefix_offsets[p1][0]];
                return FormatSectorName([p0, s0, p1, s1]);
            }
        }

        //public static string GetSystemName(ulong t)
        //{
        //    int _m = (int)(t & 7);
        //    int[] s = new int[3];
        //    t = t >> (10 - _m);
        //    for (int i = 0; i < 3; i++)
        //    {
        //        s[i] = (int)(t & ((i == 1) ? (ulong)63 : 127));
        //        t = t >> ((i == 1) ? 6 : 7) >> (7 - _m);
        //    }
        //    return FormatSectorName(GetSectorName(new Triplet(s[2], s[1], s[0])));
        //}

        #region Private methods
        private static string FormatSectorName(string[] s)
        {
            string ret;
            if (s.Length == 4 && Array.IndexOf(cx_prefixes, s[2]) > -1)
            {
                ret = s[0] + s[1] + " " + s[2] + s[3];
            }
            else
            {
                ret = "";
                for (int i = 0; i < s.Length; i++)
                {
                    ret += s[i];
                }
            }
            return ret;
        }

        //_c1_get_infix_total_run_length
        private static int C1GetInfixTotalRunLength(string s)
        {
            return (Array.IndexOf(c1_infixes_s1, s) > -1) ? c1_infix_s1_total_run_length : c1_infix_s2_total_run_length;
        }

        private static string[] C1GetInfixes(string[] frags)
        {
            string s = frags[^1];
            if (Array.IndexOf(cx_prefixes, s) > -1)
            {
                return (c1_prefix_infix_override_map.ContainsKey(s)) ? c1_infixes_s2 : c1_infixes_s1;
            }
            else if (Array.IndexOf(c1_infixes_s1, s) > -1)
            {
                return c1_infixes_s2;
            }
            else if (Array.IndexOf(c1_infixes_s2, s) > -1)
            {
                return c1_infixes_s1;
            }
            else
            {
                return [];
            }
        }


        private static string GetEntryFromOffset(int offset, List<string> keys, Dictionary<string, List<int>> d2)
        {
            foreach (string key in keys)
            {
                if (offset >= d2[key][0] && offset < (d2[key][0] + d2[key][1]))
                {
                    return key;
                }
            }
            return "";
        }

        //_get_suffixes
        private static string[] GetSuffixes(string[] frags, bool getAll = false)
        {
            string[] result;
            string wordstart = frags[0];
            string s = frags[^1];
            if (Array.IndexOf(cx_prefixes, s) > -1)
            {
                result = (c2_prefix_suffix_override_map.ContainsKey(s)) ? c2_suffixes_s2 : cx_suffixes_s1;
                wordstart = s;
            }
            else
            {
                result = (Array.IndexOf(c1_infixes_s2, s) > -1) ? cx_suffixes_s1 : c1_suffixes_s2;
            }

            if (getAll)
            {
                return result;
            }
            else
            {
                int length = GetPrefixRunLength(wordstart);
                string[] newresult = new string[length];
                Array.Copy(result, newresult, length);
                return newresult;
            }
        }

        //_get_offset_from_pos
        private static uint GetOffset(Id64XYZ t)
        {
            int ret = t.Z * GALAXYSIZE * GALAXYSIZE;
            ret += t.Y * GALAXYSIZE;
            ret += t.X;
            return (uint)ret;
        }

        // _get_c1_or_c2 returns 1 for C1, 2 for C2
        // now return true for c1, false for c2.
        private static bool IsTypeC1(uint offset)
        {
            uint key = GetJenkins32(offset);
            return ((key % 2) == 0);
        }


        private static uint GetJenkins32(uint key)
        {
            key += (key << 12);
            key &= 0xFFFFFFFF;
            key ^= (key >> 22);
            key += (key << 4);
            key &= 0xFFFFFFFF;
            key ^= (key >> 9);
            key += (key << 10);
            key &= 0xFFFFFFFF;
            key ^= (key >> 2);
            key += (key << 7);
            key &= 0xFFFFFFFF;
            key ^= (key >> 12);
            return key;
        }

        private static void Deinterleave(ulong offset, int bits, out ulong out1, out ulong out2)
        {
            out1 = 0;
            out2 = 0;
            for (int i = 0; i < bits; i += 2)
            {
                out1 |= ((offset >> i) & 1) << (i / 2);
            }
            for (int i = 1; i < bits; i += 2)
            {
                out2 |= ((offset >> i) & 1) << (i / 2);
            }
        }

        #endregion
    }
}
