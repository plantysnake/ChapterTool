// ****************************************************************************
//
// Copyright (C) 2005-2015 Doom9 & al
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// ****************************************************************************
using System.Windows.Forms;
using System.Collections.Generic;

namespace ChapterTool.Util
{
    public static class LanguageSelectionContainer
    {
        // used by all tools except MP4box
        private static readonly Dictionary<string, string> LanguagesReverseBibliographic;

        // used by MP4box
        private static readonly Dictionary<string, string> LanguagesReverseTerminology;

        // private static readonly Dictionary<string, string> languagesISO2;
        private static readonly Dictionary<string, string> LanguagesReverseISO2;

        /// <summary>
        /// uses the ISO 639-2/B language codes
        /// </summary>
        public static Dictionary<string, string> Languages { get; }

        /// <summary>
        /// uses the ISO 639-2/T language codes
        /// </summary>
        public static Dictionary<string, string> LanguagesTerminology { get; }

        private static void AddLanguage(string name, string iso3B, string iso3T, string iso2)
        {
            Languages.Add(name, iso3B);
            LanguagesReverseBibliographic.Add(iso3B, name);

            if (string.IsNullOrEmpty(iso3T))
            {
                LanguagesTerminology.Add(name, iso3B);
                LanguagesReverseTerminology.Add(iso3B, name);
            }
            else
            {
                LanguagesTerminology.Add(name, iso3T);
                LanguagesReverseTerminology.Add(iso3T, name);
            }

            if (!string.IsNullOrEmpty(iso2))
            {
                // languagesISO2.Add(name, iso2);
                LanguagesReverseISO2.Add(iso2, name);
            }
        }

        static LanguageSelectionContainer()
        {
            // http://www.loc.gov/standards/iso639-2/php/code_list.php
            // https://en.wikipedia.org/wiki/List_of_ISO_639-2_codes
            // Attention: check all tools (eac3to, mkvmerge, mediainfo, ...)

            Languages = new Dictionary<string, string>();
            LanguagesReverseBibliographic = new Dictionary<string, string>();

            LanguagesTerminology = new Dictionary<string, string>();
            LanguagesReverseTerminology = new Dictionary<string, string>();

            // languagesISO2 = new Dictionary<string, string>();
            LanguagesReverseISO2 = new Dictionary<string, string>();

            AddLanguage("Not Specified", "   ", "", "  ");
            AddLanguage("Abkhazian", "abk", "", "ab");
            AddLanguage("Achinese", "ace", "", "");
            AddLanguage("Acoli", "ach", "", "");
            AddLanguage("Adangme", "ada", "", "");
            AddLanguage("Adyghe", "ady", "", "");
            AddLanguage("Afar", "aar", "", "aa");
            AddLanguage("Afrikaans", "afr", "", "af");
            AddLanguage("Ainu", "ain", "", "");
            AddLanguage("Akan", "aka", "", "ak");
            AddLanguage("Albanian", "alb", "sqi", "sq");
            AddLanguage("Aleut", "ale", "", "");
            AddLanguage("Amharic", "amh", "", "am");
            AddLanguage("Angika", "anp", "", "");
            AddLanguage("Arabic", "ara", "", "ar");
            AddLanguage("Aragonese", "arg", "", "an");
            AddLanguage("Arapaho", "arp", "", "");
            AddLanguage("Arawak", "arw", "", "");
            AddLanguage("Armenian", "arm", "hye", "hy");
            AddLanguage("Aromanian", "rup", "", "");
            AddLanguage("Assamese", "asm", "", "as");
            AddLanguage("Asturian", "ast", "", "");
            AddLanguage("Avaric", "ava", "", "av");
            AddLanguage("Awadhi", "awa", "", "");
            AddLanguage("Aymara", "aym", "", "ay");
            AddLanguage("Azerbaijani", "aze", "", "az");
            AddLanguage("Balinese", "ban", "", "");
            AddLanguage("Baluchi", "bal", "", "");
            AddLanguage("Bambara", "bam", "", "bm");
            AddLanguage("Basa", "bas", "", "");
            AddLanguage("Bashkir", "bak", "", "ba");
            AddLanguage("Basque", "baq", "eus", "eu");
            AddLanguage("Beja", "bej", "", "");
            AddLanguage("Belarusian", "bel", "", "be");
            AddLanguage("Bemba", "bem", "", "");
            AddLanguage("Bengali", "ben", "", "bn");
            AddLanguage("Bhojpuri", "bho", "", "");
            AddLanguage("Bikol", "bik", "", "");
            AddLanguage("Bini", "bin", "", "");
            AddLanguage("Bislama", "bis", "", "bi");
            AddLanguage("Blin", "byn", "", "");
            AddLanguage("Bosnian", "bos", "", "bs");
            AddLanguage("Braj", "bra", "", "");
            AddLanguage("Breton", "bre", "", "br");
            AddLanguage("Buginese", "bug", "", "");
            AddLanguage("Bulgarian", "bul", "", "bg");
            AddLanguage("Buriat", "bua", "", "");
            AddLanguage("Burmese", "bur", "mya", "my");
            AddLanguage("Caddo", "cad", "", "");
            AddLanguage("Catalan", "cat", "", "ca");
            AddLanguage("Cebuano", "ceb", "", "");
            AddLanguage("Central Khmer", "khm", "", "km");
            AddLanguage("Chamorro", "cha", "", "ch");
            AddLanguage("Chechen", "che", "", "ce");
            AddLanguage("Cherokee", "chr", "", "");
            AddLanguage("Cheyenne", "chy", "", "");
            AddLanguage("Chichewa", "nya", "", "ny");
            AddLanguage("Chinese", "chi", "zho", "zh");
            AddLanguage("Chinook jargon", "chn", "", "");
            AddLanguage("Chipewyan", "chp", "", "");
            AddLanguage("Choctaw", "cho", "", "");
            AddLanguage("Chuukese", "chk", "", "");
            AddLanguage("Chuvash", "chv", "", "cv");
            AddLanguage("Cornish", "cor", "", "kw");
            AddLanguage("Corsican", "cos", "", "co");
            AddLanguage("Cree", "cre", "", "cr");
            AddLanguage("Creek", "mus", "", "");
            AddLanguage("Crimean Tatar", "crh", "", "");
            AddLanguage("Croatian", "hrv", "", "hr");
            AddLanguage("Czech", "cze", "ces", "cs");
            AddLanguage("Dakota", "dak", "", "");
            AddLanguage("Danish", "dan", "", "da");
            AddLanguage("Dargwa", "dar", "", "");
            AddLanguage("Delaware", "del", "", "");
            AddLanguage("Dinka", "din", "", "");
            AddLanguage("Divehi", "div", "", "dv");
            AddLanguage("Dogri", "doi", "", "");
            AddLanguage("Dogrib", "dgr", "", "");
            AddLanguage("Duala", "dua", "", "");
            AddLanguage("Dutch", "dut", "nld", "nl");
            AddLanguage("Dyula", "dyu", "", "");
            AddLanguage("Dzongkha", "dzo", "", "dz");
            AddLanguage("Eastern Frisian", "frs", "", "");
            AddLanguage("Efik", "efi", "", "");
            AddLanguage("Ekajuk", "eka", "", "");
            AddLanguage("English", "eng", "", "en");
            AddLanguage("Erzya", "myv", "", "");
            AddLanguage("Estonian", "est", "", "et");
            AddLanguage("Ewe", "ewe", "", "ee");
            AddLanguage("Ewondo", "ewo", "", "");
            AddLanguage("Fang", "fan", "", "");
            AddLanguage("Fanti", "fat", "", "");
            AddLanguage("Faroese", "fao", "", "fo");
            AddLanguage("Fijian", "fij", "", "fj");
            AddLanguage("Filipino", "fil", "", "");
            AddLanguage("Finnish", "fin", "", "fi");
            AddLanguage("Fon", "fon", "", "");
            AddLanguage("French", "fre", "fra", "fr");
            AddLanguage("Friulian", "fur", "", "");
            AddLanguage("Fulah", "ful", "", "ff");
            AddLanguage("Ga", "gaa", "", "");
            AddLanguage("Gaelic", "gla", "", "gd");
            AddLanguage("Galibi Carib", "car", "", "");
            AddLanguage("Galician", "glg", "", "gl");
            AddLanguage("Ganda", "lug", "", "lg");
            AddLanguage("Gayo", "gay", "", "");
            AddLanguage("Gbaya", "gba", "", "");
            AddLanguage("Georgian", "geo", "kat", "ka");
            AddLanguage("German", "ger", "deu", "de");
            AddLanguage("Gilbertese", "gil", "", "");
            AddLanguage("Gondi", "gon", "", "");
            AddLanguage("Gorontalo", "gor", "", "");
            AddLanguage("Grebo", "grb", "", "");
            AddLanguage("Greek", "gre", "ell", "el");
            AddLanguage("Guarani", "grn", "", "gn");
            AddLanguage("Gujarati", "guj", "", "gu");
            AddLanguage("Gwich'in", "gwi", "", "");
            AddLanguage("Haida", "hai", "", "");
            AddLanguage("Haitian", "hat", "", "ht");
            AddLanguage("Hausa", "hau", "", "ha");
            AddLanguage("Hawaiian", "haw", "", "");
            AddLanguage("Hebrew", "heb", "", "he");
            AddLanguage("Herero", "her", "", "hz");
            AddLanguage("Hiligaynon", "hil", "", "");
            AddLanguage("Hindi", "hin", "", "hi");
            AddLanguage("Hiri Motu", "hmo", "", "ho");
            AddLanguage("Hmong", "hmn", "", "");
            AddLanguage("Hungarian", "hun", "", "hu");
            AddLanguage("Hupa", "hup", "", "");
            AddLanguage("Iban", "iba", "", "");
            AddLanguage("Icelandic", "ice", "isl", "is");
            AddLanguage("Igbo", "ibo", "", "ig");
            AddLanguage("Iloko", "ilo", "", "");
            AddLanguage("Inari Sami", "smn", "", "");
            AddLanguage("Indonesian", "ind", "", "id");
            AddLanguage("Ingush", "inh", "", "");
            AddLanguage("Inuktitut", "iku", "", "iu");
            AddLanguage("Inupiaq", "ipk", "", "ik");
            AddLanguage("Irish", "gle", "", "ga");
            AddLanguage("Italian", "ita", "", "it");
            AddLanguage("Japanese", "jpn", "", "ja");
            AddLanguage("Javanese", "jav", "", "jv");
            AddLanguage("Judeo-Arabic", "jrb", "", "");
            AddLanguage("Judeo-Persian", "jpr", "", "");
            AddLanguage("Kabardian", "kbd", "", "");
            AddLanguage("Kabyle", "kab", "", "");
            AddLanguage("Kachin", "kac", "", "");
            AddLanguage("Kalaallisut", "kal", "", "kl");
            AddLanguage("Kalmyk", "xal", "", "");
            AddLanguage("Kamba", "kam", "", "");
            AddLanguage("Kannada", "kan", "", "kn");
            AddLanguage("Kanuri", "kau", "", "kr");
            AddLanguage("Karachay-Balkar", "krc", "", "");
            AddLanguage("Kara-Kalpak", "kaa", "", "");
            AddLanguage("Karelian", "krl", "", "");
            AddLanguage("Kashmiri", "kas", "", "ks");
            AddLanguage("Kashubian", "csb", "", "");
            AddLanguage("Kazakh", "kaz", "", "kk");
            AddLanguage("Khasi", "kha", "", "");
            AddLanguage("Kikuyu", "kik", "", "ki");
            AddLanguage("Kimbundu", "kmb", "", "");
            AddLanguage("Kinyarwanda", "kin", "", "rw");
            AddLanguage("Kirghiz", "kir", "", "ky");
            AddLanguage("Komi", "kom", "", "kv");
            AddLanguage("Kongo", "kon", "", "kg");
            AddLanguage("Konkani", "kok", "", "");
            AddLanguage("Korean", "kor", "", "ko");
            AddLanguage("Kosraean", "kos", "", "");
            AddLanguage("Kpelle", "kpe", "", "");
            AddLanguage("Kuanyama", "kua", "", "kj");
            AddLanguage("Kumyk", "kum", "", "");
            AddLanguage("Kurdish", "kur", "", "ku");
            AddLanguage("Kurukh", "kru", "", "");
            AddLanguage("Kutenai", "kut", "", "");
            AddLanguage("Ladino", "lad", "", "");
            AddLanguage("Lahnda", "lah", "", "");
            AddLanguage("Lamba", "lam", "", "");
            AddLanguage("Lao", "lao", "", "lo");
            AddLanguage("Latvian", "lav", "", "lv");
            AddLanguage("Lezghian", "lez", "", "");
            AddLanguage("Limburgan", "lim", "", "li");
            AddLanguage("Lingala", "lin", "", "ln");
            AddLanguage("Lithuanian", "lit", "", "lt");
            AddLanguage("Low German", "nds", "", "");
            AddLanguage("Lower Sorbian", "dsb", "", "");
            AddLanguage("Lozi", "loz", "", "");
            AddLanguage("Luba-Katanga", "lub", "", "lu");
            AddLanguage("Luba-Lulua", "lua", "", "");
            AddLanguage("Luiseno", "lui", "", "");
            AddLanguage("Lule Sami", "smj", "", "");
            AddLanguage("Lunda", "lun", "", "");
            AddLanguage("Luo", "luo", "", "");
            AddLanguage("Lushai", "lus", "", "");
            AddLanguage("Luxembourgish", "ltz", "", "lb");
            AddLanguage("Macedonian", "mac", "mkd", "mk");
            AddLanguage("Madurese", "mad", "", "");
            AddLanguage("Magahi", "mag", "", "");
            AddLanguage("Maithili", "mai", "", "");
            AddLanguage("Makasar", "mak", "", "");
            AddLanguage("Malagasy", "mlg", "", "mg");
            AddLanguage("Malay", "may", "msa", "ms");
            AddLanguage("Malayalam", "mal", "", "ml");
            AddLanguage("Maltese", "mlt", "", "mt");
            AddLanguage("Manchu", "mnc", "", "");
            AddLanguage("Mandar", "mdr", "", "");
            AddLanguage("Mandingo", "man", "", "");
            AddLanguage("Manipuri", "mni", "", "");
            AddLanguage("Manx", "glv", "", "gv");
            AddLanguage("Maori", "mao", "mri", "mi");
            AddLanguage("Mapudungun", "arn", "", "");
            AddLanguage("Marathi", "mar", "", "mr");
            AddLanguage("Mari", "chm", "", "");
            AddLanguage("Marshallese", "mah", "", "mh");
            AddLanguage("Marwari", "mwr", "", "");
            AddLanguage("Masai", "mas", "", "");
            AddLanguage("Mende", "men", "", "");
            AddLanguage("Mi'kmaq", "mic", "", "");
            AddLanguage("Minangkabau", "min", "", "");
            AddLanguage("Mirandese", "mwl", "", "");
            AddLanguage("Mohawk", "moh", "", "");
            AddLanguage("Moksha", "mdf", "", "");
            AddLanguage("Moldavian", "mol", "", "mo");
            AddLanguage("Mongo", "lol", "", "");
            AddLanguage("Mongolian", "mon", "", "mn");
            AddLanguage("Mossi", "mos", "", "");
            AddLanguage("Nauru", "nau", "", "na");
            AddLanguage("Navajo", "nav", "", "nv");
            AddLanguage("Ndebele, North", "nde", "", "nd");
            AddLanguage("Ndebele, South", "nbl", "", "nr");
            AddLanguage("Ndonga", "ndo", "", "ng");
            AddLanguage("Neapolitan", "nap", "", "");
            AddLanguage("Nepal Bhasa", "new", "", "");
            AddLanguage("Nepali", "nep", "", "ne");
            AddLanguage("Nias", "nia", "", "");
            AddLanguage("Niuean", "niu", "", "");
            AddLanguage("N'Ko", "nqo", "", "");
            AddLanguage("Nogai", "nog", "", "");
            AddLanguage("Northern Frisian", "frr", "", "");
            AddLanguage("Northern Sami", "sme", "", "se");
            AddLanguage("Norwegian", "nor", "", "no");
            AddLanguage("norwegian bokmål", "nob", "", "nb");
            AddLanguage("Norwegian Nynorsk", "nno", "", "nn");
            AddLanguage("Nyamwezi", "nym", "", "");
            AddLanguage("Nyankole", "nyn", "", "");
            AddLanguage("Nyoro", "nyo", "", "");
            AddLanguage("Nzima", "nzi", "", "");
            AddLanguage("Occitan", "oci", "", "oc");
            AddLanguage("Ojibwa", "oji", "", "oj");
            AddLanguage("Oriya", "ori", "", "or");
            AddLanguage("Oromo", "orm", "", "om");
            AddLanguage("Osage", "osa", "", "");
            AddLanguage("Ossetian", "oss", "", "os");
            AddLanguage("Palauan", "pau", "", "");
            AddLanguage("Pampanga", "pam", "", "");
            AddLanguage("Pangasinan", "pag", "", "");
            AddLanguage("Panjabi", "pan", "", "pa");
            AddLanguage("Papiamento", "pap", "", "");
            AddLanguage("Pedi", "nso", "", "");
            AddLanguage("Persian", "per", "fas", "fa");
            AddLanguage("Pohnpeian", "pon", "", "");
            AddLanguage("Polish", "pol", "", "pl");
            AddLanguage("Portuguese", "por", "", "pt");
            AddLanguage("Pushto", "pus", "", "ps");
            AddLanguage("Quechua", "que", "", "qu");
            AddLanguage("Rajasthani", "raj", "", "");
            AddLanguage("Rapanui", "rap", "", "");
            AddLanguage("Rarotongan", "rar", "", "");
            AddLanguage("Romanian", "rum", "ron", "ro");
            AddLanguage("Romansh", "roh", "", "rm");
            AddLanguage("Romany", "rom", "", "");
            AddLanguage("Rundi", "run", "", "rn");
            AddLanguage("Russian", "rus", "", "ru");
            AddLanguage("Samoan", "smo", "", "sm");
            AddLanguage("Sandawe", "sad", "", "");
            AddLanguage("Sango", "sag", "", "sg");
            AddLanguage("Santali", "sat", "", "");
            AddLanguage("Sardinian", "srd", "", "sc");
            AddLanguage("Sasak", "sas", "", "");
            AddLanguage("Scots", "sco", "", "");
            AddLanguage("Selkup", "sel", "", "");
            AddLanguage("Serbian", "srp", "", "sr");
            AddLanguage("Serer", "srr", "", "");
            AddLanguage("Shan", "shn", "", "");
            AddLanguage("Shona", "sna", "", "sn");
            AddLanguage("Sichuan Yi", "iii", "", "ii");
            AddLanguage("Sicilian", "scn", "", "");
            AddLanguage("Sidamo", "sid", "", "");
            AddLanguage("Siksika", "bla", "", "");
            AddLanguage("Sindhi", "snd", "", "sd");
            AddLanguage("Sinhala", "sin", "", "si");
            AddLanguage("Skolt Sami", "sms", "", "");
            AddLanguage("Slave (Athapascan)", "den", "", "");
            AddLanguage("Slovak", "slo", "slk", "sk");
            AddLanguage("Slovenian", "slv", "", "sl");
            AddLanguage("Somali", "som", "", "so");
            AddLanguage("Soninke", "snk", "", "");
            AddLanguage("Sotho, Southern", "sot", "", "st");
            AddLanguage("Southern Altai", "alt", "", "");
            AddLanguage("Southern Sami", "sma", "", "");
            AddLanguage("Spanish", "spa", "", "es");
            AddLanguage("Sranan Tongo", "srn", "", "");
            AddLanguage("Standard Moroccan Tamazight", "zgh", "", "");
            AddLanguage("Sukuma", "suk", "", "");
            AddLanguage("Sundanese", "sun", "", "su");
            AddLanguage("Susu", "sus", "", "");
            AddLanguage("Swahili", "swa", "", "sw");
            AddLanguage("Swati", "ssw", "", "ss");
            AddLanguage("Swedish", "swe", "", "sv");
            AddLanguage("Swiss German", "gsw", "", "");
            AddLanguage("Syriac", "syr", "", "");
            AddLanguage("Tagalog", "tgl", "", "tl");
            AddLanguage("Tahitian", "tah", "", "ty");
            AddLanguage("Tajik", "tgk", "", "tg");
            AddLanguage("Tamashek", "tmh", "", "");
            AddLanguage("Tamil", "tam", "", "ta");
            AddLanguage("Tatar", "tat", "", "tt");
            AddLanguage("Telugu", "tel", "", "te");
            AddLanguage("Tereno", "ter", "", "");
            AddLanguage("Tetum", "tet", "", "");
            AddLanguage("Thai", "tha", "", "th");
            AddLanguage("Tibetan", "tib", "bod", "bo");
            AddLanguage("Tigre", "tig", "", "");
            AddLanguage("Tigrinya", "tir", "", "ti");
            AddLanguage("Timne", "tem", "", "");
            AddLanguage("Tiv", "tiv", "", "");
            AddLanguage("Tlingit", "tli", "", "");
            AddLanguage("Tok Pisin", "tpi", "", "");
            AddLanguage("Tokelau", "tkl", "", "");
            AddLanguage("Tonga (Nyasa)", "tog", "", "");
            AddLanguage("Tonga (Tonga Islands)", "ton", "", "to");
            AddLanguage("Tsimshian", "tsi", "", "");
            AddLanguage("Tsonga", "tso", "", "ts");
            AddLanguage("Tswana", "tsn", "", "tn");
            AddLanguage("Tumbuka", "tum", "", "");
            AddLanguage("Turkish", "tur", "", "tr");
            AddLanguage("Turkmen", "tuk", "", "tk");
            AddLanguage("Tuvalu", "tvl", "", "");
            AddLanguage("Tuvinian", "tyv", "", "");
            AddLanguage("Twi", "twi", "", "tw");
            AddLanguage("Udmurt", "udm", "", "");
            AddLanguage("Uighur", "uig", "", "ug");
            AddLanguage("Ukrainian", "ukr", "", "uk");
            AddLanguage("Umbundu", "umb", "", "");
            AddLanguage("Uncoded languages", "mis", "", "");
            AddLanguage("Undetermined", "und", "", "");
            AddLanguage("Upper Sorbian", "hsb", "", "");
            AddLanguage("Urdu", "urd", "", "ur");
            AddLanguage("Uzbek", "uzb", "", "uz");
            AddLanguage("Vai", "vai", "", "");
            AddLanguage("Venda", "ven", "", "ve");
            AddLanguage("Vietnamese", "vie", "", "vi");
            AddLanguage("Votic", "vot", "", "");
            AddLanguage("Walloon", "wln", "", "wa");
            AddLanguage("Waray", "war", "", "");
            AddLanguage("Washo", "was", "", "");
            AddLanguage("Welsh", "wel", "cym", "cy");
            AddLanguage("Western Frisian", "fry", "", "fy");
            AddLanguage("Wolaitta", "wal", "", "");
            AddLanguage("Wolof", "wol", "", "wo");
            AddLanguage("Xhosa", "xho", "", "xh");
            AddLanguage("Yakut", "sah", "", "");
            AddLanguage("Yao", "yao", "", "");
            AddLanguage("Yapese", "yap", "", "");
            AddLanguage("Yiddish", "yid", "", "yi");
            AddLanguage("Yoruba", "yor", "", "yo");
            AddLanguage("Zapotec", "zap", "", "");
            AddLanguage("Zaza", "zza", "", "");
            AddLanguage("Zenaga", "zen", "", "");
            AddLanguage("Zhuang", "zha", "", "za");
            AddLanguage("Zulu", "zul", "", "zu");
            AddLanguage("Zuni", "zun", "", "");
        }

        ///<summary>
        ///Convert the 2 or 3 char string to the full language name
        ///</summary>
        public static string LookupISOCode(string code)
        {
            switch (code.Length)
            {
                case 2:
                    if (LanguagesReverseISO2.ContainsKey(code))
                        return LanguagesReverseISO2[code];
                    break;
                case 3:
                    if (LanguagesReverseBibliographic.ContainsKey(code))
                        return LanguagesReverseBibliographic[code];
                    if (LanguagesReverseTerminology.ContainsKey(code))
                        return LanguagesReverseTerminology[code];
                    break;
            }
            return "";
        }

        public static bool IsLanguageAvailable(string language) => Languages.ContainsKey(language);

        public static void LoadLang(ComboBox target)
        {
            target.Items.Add("----常用----"       );
            target.Items.Add("und (Undetermined)" );
            target.Items.Add("eng (English)"      );
            target.Items.Add("jpn (Japanese)"     );
            target.Items.Add("chi (Chinese)"      );
            target.Items.Add("----全部----"       );
            foreach (var language in Languages)
            {
                target.Items.Add($"{language.Value} ({language.Key})");
            }
        }
    }
}
