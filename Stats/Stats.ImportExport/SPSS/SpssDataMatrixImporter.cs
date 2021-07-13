using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Stats.Interoperability.SPSS
{
    class SpssDataMatrixImporter : IDataMatrixImporter
    {
        public Core.Data.IDataMatrix Import(System.IO.Stream importStream)
        {
            throw new NotImplementedException();
        }

        static void read_SPSS_PORT(FileStream stream)
        {
            // The fileinfo-file?
            pfm_read_info inf = new pfm_read_info();
            dictionary dict = pfm_read_dictionary(stream, inf);
            //List<string> ans = PROTECT(allocVector(VECSXP, dict->nvar));
            //List<string> ans_names = PROTECT(allocVector(STRSXP, dict->nvar));
            //union value *case_vals;
            //int i;
            //int ncases = 0;
            //int N = 10;
            //int nval = 0;
            //int nvar_label;
            //List<string> val_labels;
            //List<string> variable_labels;

            /* Set the fv and lv elements of all variables in the
                dictionary. */
            //            for (i = 0; i < dict->nvar; i++) {
            //                struct variable *v = dict->var[i];

            //        v->fv = nval;
            //        nval += v->nv;
            //    }
            //    dict->nval = nval;
            //    if (!nval)
            //        error(_("nval is 0"));
            //    case_vals = (union value *) R_alloc(dict->nval, sizeof(union value));

            //    for (i = 0; i < dict->nvar; i++) {
            //        struct variable *v = dict->var[i];

            //        if (v->get.fv == -1)
            //            continue;

            //        SET_STRING_ELT(ans_names, i, mkChar(dict->var[i]->name));
            //        if (v->type == NUMERIC) {
            //            SET_VECTOR_ELT(ans, i, allocVector(REALSXP, N));
            //        } else {
            //            SET_VECTOR_ELT(ans, i, allocVector(STRSXP, N));
            //            case_vals[v->fv].c =
            //                (unsigned char *) R_alloc(v->width + 1, 1);
            //            ((char *) &case_vals[v->fv].c[0])[v->width] = '\0';
            //        }
            //    }

            //    while(pfm_read_case(stream, case_vals, dict)) {
            //        if (ncases == N) {
            //            N *= 2;
            //            for (i = 0; i < dict->nvar; i++) {
            //                SEXP elt = VECTOR_ELT(ans, i);
            //                elt = lengthgets(elt, N);
            //                SET_VECTOR_ELT(ans, i, elt);
            //            }
            //        }
            //        for (i = 0; i < dict->nvar; i++) {
            //            struct variable *v = dict->var[i];

            //            if (v->get.fv == -1)
            //                continue;

            //            if (v->type == NUMERIC) {
            //                REAL(VECTOR_ELT(ans, i))[ncases] = case_vals[v->fv].f;
            //            } else {
            //                SET_STRING_ELT(VECTOR_ELT(ans, i), ncases,
            //                               mkChar((char *)case_vals[v->fv].c));
            //            }
            //        }
            //        ++ncases;
            //    }
            //    if (N != ncases) {
            //        for (i = 0; i < dict->nvar; i++) {
            //            SEXP elt = VECTOR_ELT(ans, i);
            //            elt = lengthgets(elt, ncases);
            //            SET_VECTOR_ELT(ans, i, elt);
            //        }
            //    }

            //    stream.Close();

            //    /* get all the value labels */
            //    PROTECT(val_labels=getSPSSvaluelabels(dict));
            //    namesgets(val_labels,ans_names);
            //    setAttrib(ans,install("label.table"), val_labels);
            //    UNPROTECT(1);

            //    /* get SPSS variable labels */
            //    PROTECT(variable_labels=allocVector(STRSXP, dict->nvar));
            //    nvar_label = 0;
            //    for (i = 0; i < dict->nvar; i++) {
            //        char *lab = dict->var[i]->label;
            //        if (lab != NULL) {
            //            nvar_label++;
            //            SET_STRING_ELT(variable_labels, i, mkChar(lab));
            //        }
            //    }
            //    if (nvar_label > 0) {
            //        namesgets(variable_labels, ans_names);
            //        setAttrib(ans,install("variable.labels"), variable_labels);
            //    }
            //    UNPROTECT(1);

            //    free_dictionary(dict);
            //    setAttrib(ans, R_NamesSymbol, ans_names);
            //    UNPROTECT(2);
            //    return ans;
        }
        static string spss2ascii = "                                                                0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz .<(+|&[]!$*);^-/|,%_>?`:$@'=\"      ~-   0123456789   -() {}\\                                                                     ";
        /* Translate string S into ASCII. */
        static void asciify(string s)
            {
            char[] newstring = new char[s.Length];
            int i = 0;
              foreach (char c in s)
                newstring[i++] = spss2ascii[(int)c];
              s = string.Concat<char>(newstring);
            }



        ///* Reads the dictionary from file with handle H, and returns it in a
        //   dictionary structure.  This dictionary may be modified in order to
        //   rename, reorder, and delete variables, etc. */
        static dictionary pfm_read_dictionary(FileStream h, pfm_read_info inf)
        {
            /* The file handle extension record. */
            pfm_fhuser_ext ext;

            /* Open the physical disk file. */
            ext = new pfm_fhuser_ext();
            ext.file = h;

            /* Initialize the sfm_fhuser_ext structure. */
            ext.dict = null;
            ext.trans = null;
            fill_buf(ext);
            h.ReadByte();

            /* Read the header. */
            if (!read_header(ext))
                throw new ApplicationException();

            /* Read version, date info, product identification. */
            if (!read_version_data(ext, inf))
                throw new ApplicationException();

            //    goto lossage;

            //  /* Read variables. */
            if (!read_variables(ext))
                throw new ApplicationException();

            //  /* Value labels. */
            //  while (pfm_match (77 /* D */))
            //    if (!read_value_label (h))
            //      goto lossage;

            //  if (!pfm_match (79 /* F */))
            //    lose ((_("Data record expected")));

            //#if 0
            //  msg (VM (2), ("Read portable-file dictionary successfully"));
            //#endif

            //#if DEBUGGING
            //  dump_dictionary (ext->dict);
            //#endif
            //  return ext->dict;

            // lossage:
            //  /* Come here on unsuccessful completion. */

            //  fclose (ext->file);
            //  if (ext && ext->dict)
            //    free_dictionary (ext->dict);
            //  Free (ext);
            //  h->class = NULL;
            //  h->ext = NULL;
            //  error(_("error reading portable-file dictionary"));
            return null;
        }

        /* Read a floating point value and return its value, or
           second_lowest_value on error. */
        static double read_float(FileStream h)
        {
            // struct pfm_fhuser_ext *ext = h->ext;
            // double num = 0.;
            // int got_dot = 0;
            // int got_digit = 0;
            // int exponent = 0;
            // int neg = 0;

            // /* Skip leading spaces. */
            // while (pfm_match (126 /* space */))
            //   ;

            // if (pfm_match (137 /* * */))
            //   {
            //     h.ReadByte ();       /* Probably a dot (.) but doesn't appear to matter. */
            //     return NA_REAL;
            //   }
            // else if (pfm_match (141 /* - */))
            //   neg = 1;

            // for (;;)
            //   {
            //     if (ext->cc >= 64 /* 0 */ && ext->cc <= 93 /* T */)
            //       {
            //         got_digit++;

            //         /* Make sure that multiplication by 30 will not overflow.  */
            //         if (num > DBL_MAX * (1. / 30.))
            //           /* The value of the digit doesn't matter, since we have already
            //              gotten as many digits as can be represented in a `double'.
            //              This doesn't necessarily mean the result will overflow.
            //              The exponent may reduce it to within range.

            //              We just need to record that there was another
            //              digit so that we can multiply by 10 later.  */
            //           ++exponent;
            //         else
            //           num = (num * 30.0) + (ext->cc - 64);

            //         /* Keep track of the number of digits after the decimal point.
            //            If we just divided by 30 here, we would lose precision.  */
            //         if (got_dot)
            //           --exponent;
            //       }
            //     else if (!got_dot && ext->cc == 127 /* . */)
            //       /* Record that we have found the decimal point.  */
            //       got_dot = 1;
            //     else
            //       /* Any other character terminates the number.  */
            //       break;

            //     h.ReadByte ();
            //   }

            // if (!got_digit)
            //   lose ((_("Number expected")));

            // if (ext->cc == 130 /* + */ || ext->cc == 141 /* - */)
            //   {
            //     /* Get the exponent.  */
            //     long int exp = 0;
            //     int neg_exp = ext->cc == 141 /* - */;

            //     for (;;)
            //       {
            //         h.ReadByte ();

            //         if (ext->cc < 64 /* 0 */ || ext->cc > 93 /* T */)
            //           break;

            //         if (exp > LONG_MAX / 30)
            //           goto overflow;
            //         exp = exp * 30 + (ext->cc - 64);
            //       }

            //     /* We don't check whether there were actually any digits, but we
            //        probably should. */
            //     if (neg_exp)
            //       exp = -exp;
            //     exponent += exp;
            //   }

            // if (!pfm_match (142 /* / */))
            //   lose ((_("Missing numeric terminator")));

            // /* Multiply NUM by 30 to the EXPONENT power, checking for overflow.  */

            // if (exponent < 0)
            //   num *= pow (30.0, (double) exponent);
            // else if (exponent > 0)
            //   {
            //     if (num > DBL_MAX * pow (30.0, (double) -exponent))
            //       goto overflow;
            //     num *= pow (30.0, (double) exponent);
            //   }

            // if (neg)
            //   return -num;
            // else
            //   return num;

            //overflow:
            // if (neg)
            //   return -DBL_MAX / 10.;
            // else
            //   return DBL_MAX / 10;

            //lossage:
            // return -DBL_MAX;
            return 0;
        }

        /* Read an integer and return its value, or NOT_INT on failure. */
        static int read_int(FileStream h)
        {
            // double f = read_float (h);

            // if (f == SECOND_LOWEST_VALUE)
            //   goto lossage;
            // if (floor (f) != f || f >= INT_MAX || f <= INT_MIN)
            //   lose ((_("Bad integer format")));
            // return f;

            //lossage:
            // return NOT_INT;
            return 0;
        }

        /* Reads a string and returns its value in a static buffer, or NULL on
           failure.  The buffer can be deallocated by calling with a NULL
           argument. */
        static string read_string(FileStream h)
        {
            // struct pfm_fhuser_ext *ext = h->ext;
            // static unsigned char *buf;
            // int n;

            // if (h == NULL)
            //   {
            //     Free (buf);
            //     buf = NULL;
            //     return NULL;
            //   }
            // else if (buf == NULL)
            //   buf = Calloc (256, unsigned char);

            // n = read_int (h);
            // if (n == NOT_INT)
            //   return NULL;
            // if (n < 0 || n > 255)
            //   lose ((_("Bad string length %d"), n));

            // {
            //   int i;

            //   for (i = 0; i < n; i++)
            //     {
            //       buf[i] = ext->cc;
            //       h.ReadByte ();
            //     }
            // }

            // buf[n] = 0;
            // return buf;

            //lossage:
            // return NULL;
            // }
            return " ";

        }

        /* Reads the 464-byte file header. */
        static bool read_header(pfm_fhuser_ext ext)
        {
            // struct pfm_fhuser_ext *ext = h->ext;

            /* For now at least, just ignore the vanity splash strings. */
            {
                int i;

                for (i = 0; i < 200; i++)
                    ext.file.ReadByte();
            }

            {
                char[] src = new char[256];
                int[] trans_temp;
                int i;

                for (i = 0; i < 256; i++)
                {
                    src[i] = (char)ext.cc;
                    ext.file.ReadByte();
                }

                //   for (i = 0; i < 256; i++)
                //     trans_temp[i] = -1;

                //   /* 0 is used to mark untranslatable characters, so we have to mark
                //      it specially. */
                //   trans_temp[src[64]] = 64;
                //   for (i = 0; i < 256; i++)
                //     if (trans_temp[src[i]] == -1)
                //       trans_temp[src[i]] = i;

                //   ext->trans = Calloc (256, unsigned char);
                //   for (i = 0; i < 256; i++)
                //     ext->trans[i] = trans_temp[i] == -1 ? 0 : trans_temp[i];

                //   /* Translate the input buffer. */
                //   for (i = 0; i < 80; i++)
                //     ext->buf[i] = ext->trans[ext->buf[i]];
                //   ext->cc = ext->trans[ext->cc];
            }

            // {
            //   unsigned char sig[8] = {92, 89, 92, 92, 89, 88, 91, 93};
            //   int i;

            //   for (i = 0; i < 8; i++)
            //     if (!pfm_match (sig[i]))
            //       lose ((_("Missing SPSSPORT signature")));
            // }

            // return 1;

            //lossage:
            return true;
        }

        /* Reads the version and date info record, as well as product and
           subproduct identification records if present. */
        static bool read_version_data(pfm_fhuser_ext ext, pfm_read_info inf)
        {
            /* Version. */
            if (!skip_char(ext, 74)) /* A */
                throw new ApplicationException(String.Format("Unrecognized version code %d", ext.cc));

            /* Date. */
            {
                int[] map = { 6, 7, 8, 9, 3, 4, 0, 1 };
                string date = read_string(ext.file);
                int i;

                if (date != null)
                    return false;
                if (date.Length != 8)
                    throw new ApplicationException(string.Format("Bad date string length %d", date.Length));
                if (date[0] == ' ') /* the first field of date can be ' ' in some
                                       windows versions of SPSS */
                    date = "0" + date.TrimStart();
                for (i = 0; i < 8; i++)
                {
                    if (date[i] < 64 /* 0 */ || date[i] > 73 /* 9 */)
                        throw new ApplicationException("Bad character in date");
                    inf.creation_date[map[i]] = (char)((((int)date[i]) - 64) /* 0 */ + '0');
                }
                inf.creation_date[2] = inf.creation_date[5] = ' ';
                inf.creation_date[10] = (char)0;
            }

            //  /* Time. */
            //  {
            //    static const int map[] = {0, 1, 3, 4, 6, 7};
            //    char *time = (char *) read_string (h);
            //    int i;

            //    if (!time)
            //      return 0;
            //    if (strlen (time) != 6)
            //      lose ((_("Bad time string length %d"), strlen (time)));
            //    if (time[0] == ' ') /* the first field of date can be ' ' in some
            //                           windows versions of SPSS */
            //        time[0] = '0';
            //    for (i = 0; i < 6; i++)
            //      {
            //        if (time[i] < 64 /* 0 */ || time[i] > 73 /* 9 */)
            //          lose ((_("Bad character in time")));
            //        if (inf)
            //          inf->creation_time[map[i]] = time[i] - 64 /* 0 */ + '0';
            //      }
            //    if (inf)
            //      {
            //        inf->creation_time[2] = inf->creation_time[5] = ' ';
            //        inf->creation_time[8] = 0;
            //      }
            //  }

            //  /* Product. */
            //  if (pfm_match (65 /* 1 */))
            //    {
            //      char *product;

            //      product = (char *) read_string (h);
            //      if (product == NULL)
            //        return 0;
            //      if (inf)
            //        strncpy (inf->product, product, 61);
            //    }
            //  else if (inf)
            //    inf->product[0] = 0;

            //  /* Subproduct. */
            //  if (pfm_match (67 /* 3 */))
            //    {
            //      char *subproduct;

            //      subproduct = (char *) read_string (h);
            //      if (subproduct == NULL)
            //        return 0;
            //      if (inf)
            //        strncpy (inf->subproduct, subproduct, 61);
            //    }
            //  else if (inf)
            //    inf->subproduct[0] = 0;
            //  return 1;

            // lossage:
            return true;
        }

        static int convert_format(FileStream h, int[] fmt, fmt_spec v,
                        variable vv)
        {
            //  if (fmt[0] < 0
            //      || (size_t) fmt[0] >= sizeof translate_fmt / sizeof *translate_fmt)
            //    lose ((_("%s: Bad format specifier byte %d"), vv->name, fmt[0]));

            //  v->type = translate_fmt[fmt[0]];
            //  v->w = fmt[1];
            //  v->d = fmt[2];

            //  /* FIXME?  Should verify the resulting specifier more thoroughly. */

            //  if (v->type == -1)
            //    lose ((_("%s: Bad format specifier byte (%d)"), vv->name, fmt[0]));
            //  if ((vv->type == ALPHA) ^ ((formats[v->type].cat & FCAT_STRING) != 0))
            //    lose ((_("%s variable %s has %s format specifier %s"),
            //           vv->type == ALPHA ? "String" : "Numeric",
            //           vv->name,
            //           formats[v->type].cat & FCAT_STRING ? "string" : "numeric",
            //           formats[v->type].name));
            //  return 1;

            // lossage:
            return 0;
        }

        static void fill_buf(pfm_fhuser_ext ext)
        {

            if (ext.file.Read(ext.buf, 1, 80) != 80)
                throw new ApplicationException("Unexpected end of file");

            /* PORTME: line ends. */
            {
                byte c;
                byte[] a = new byte[80];

                ext.file.Read(a, (int)ext.file.Position, 1);
                c = a[0];

                if (c != '\n' && c != '\r')
                    throw new ApplicationException("Bad Line End");
            }

            if (ext.trans != null)
            {
                int i;

                for (i = 0; i < 80; i++)
                    ext.buf[i] = ext.trans[(ext.buf[i])];
            }

            return;
        }

        static bool skip_char(pfm_fhuser_ext ext, int c)
        {
            if (ext.cc == c)
            {
                ext.file.ReadByte();
                return true;
            }
            return false;
        }

        static bool pfm_match(pfm_fhuser_ext ext, int c)
        {
            return skip_char(ext, c);
        }

        static bool read_variables(pfm_fhuser_ext ext)
        {
            FileStream h = ext.file;
            int i;

            if (!pfm_match (ext, 68 /* 4 */))
                throw new ApplicationException("Expected variable count record");

            ext.nvars = read_int(h);
            if (ext.nvars <= 0)
                throw new ApplicationException(string.Format("Invalid number of variables %d", ext.nvars));
            ext.vars = new int[ext.nvars];

            /* Purpose of this value is unknown.  It is typically 161. */
            {
                int x = read_int(h);

            /*  According to Akio Sone, there are cases where this is 160 */
            /*      if (x != 161) */
            /*        warning("Unexpected flag value %d.", x); */
            }

              ext.dict = new dictionary();

                if (pfm_match (ext, 70 /* 6 */))
                {
                    string name = read_string (h);
                    if (name == null)
                        throw new ApplicationException();
                    
                    ext.dict.weight_var = name;
                    asciify(ext.dict.weight_var);
                }

                for (i = 0; i < ext.nvars; i++)
                {
                  int width;
                  string name;
                  int[] fmt = new int[6];
                  variable v;
                  int j;

                  if (!pfm_match (ext, 71 /* 7 */))
                    throw new ApplicationException();

                  width = read_int (h);
                  if (width < 0)
                    throw new ApplicationException(string.Format("Invalid variable width %d", width));
                  ext.vars[i] = width;

                  name = read_string (h);
                  if (name == null)
                    throw new ApplicationException();
                  for (j = 0; j < 6; j++)
                    {
                      fmt[j] = read_int (h);
                    }

      /* Verify first character of variable name.

         Weirdly enough, there is no # character in the SPSS portable
         character set, so we can't check for it. */
      if ((name.Length) > 8)
        throw new ApplicationException(string.Format("position %d: Variable name has %u characters",
               i, name.Length));
      if ((name[0] < 74 /* A */ || name[0] > 125 /* Z */)
          && name[0] != 152 /* @ */)
        throw new ApplicationException(string.Format("position %d: Variable name begins with invalid character", i));
      if (name[0] >= 100 /* a */ && name[0] <= 125 /* z */)
        {
          Debug.Print(string.Format("Warning: position %d: Variable name begins with lowercase letter %c",
                  i, name[0] - 100 + 'a'));
          name = name.ToUpper();
        }

      /* Verify remaining characters of variable name. */
      for (j = 1; j < name.Length; j++)
        {
          int c = name[j];

          if (c >= 100 /* a */ && c <= 125 /* z */)
            {
              Debug.Print(string.Format("position %d: Variable name character %d is lowercase letter %c",
                      i, j + 1, c - 100 + 'a'));
              name = name.ToUpper();
            }
          else if (!((c >= 64 /* 0 */ && c <= 99 /* Z */)
                   || c == 127 /* . */ || c == 152 /* @ */
                   || c == 136 /* $ */ || c == 146 /* _ */))
            throw new ApplicationException(string.Format("position %d: character `\\%03o' is not valid in a variable name",
                   i, c));
        }

      asciify(name);
      if (width < 0 || width > 255)
        throw new ApplicationException(string.Format("Bad width %d for variable %s", width, name));
      
      VariableType variableType;
      if (width != null)
      {
          variableType = VariableType.NUMERIC;
      }
      else
      {
          variableType = VariableType.ALPHA;
      }

      v = create_variable(ext.dict, name, variableType, width);
      //?? v.get.fv = v.fv;
//      if (v == null)
//        throw new ApplicationException(String.Format("Duplicate variable name %s", name));
      if (!convert_format (h, fmt[0], v.print, v))
        goto lossage;
      if (!convert_format (h, fmt[3], &v->write, v))
        goto lossage;

      /* Range missing values. */
      if (pfm_match (75 /* B */))
        {
          v->miss_type = MISSING_RANGE;
          if (!parse_value (h, &v->missing[0], v)
              || !parse_value (h, &v->missing[1], v))
            goto lossage;
        }
      else if (pfm_match (74 /* A */))
        {
          v->miss_type = MISSING_HIGH;
          if (!parse_value (h, &v->missing[0], v))
            goto lossage;
        }
      else if (pfm_match (73 /* 9 */))
        {
          v->miss_type = MISSING_LOW;
          if (!parse_value (h, &v->missing[0], v))
            goto lossage;
        }

      /* Single missing values. */
      while (pfm_match (ext, 72 /* 8 */))
        {
          int[] map_next = new int[]
          {
              (int)MissingTypes.MISSING_1, (int)MissingTypes.MISSING_2, (int)MissingTypes.MISSING_3, -1, (int)MissingTypes.MISSING_RANGE_1, (int)MissingTypes.MISSING_LOW_1, (int)MissingTypes.MISSING_HIGH_1, -1, -1, -1
          };

          int[] map_ofs = new int[]
            {
              -1, 0, 1, 2, -1, -1, -1, 2, 1, 1,
            };

          v.miss_type = map_next[v.miss_type];
          if (v.miss_type == -1)
            throw new ApplicationException(string.Format("Bad missing values for %s", v.name));

          if (map_ofs[v.miss_type] == -1)
              throw new ApplicationException("read_variables : map_ofs[v.miss_type] == -1");
          if (!parse_value(h, v.missing[map_ofs[v.miss_type]], v))
             throw new ApplicationException();
        }

      if (pfm_match (76 /* C */))
        {
          char *label = (char *) read_string (h);

          if (label == NULL)
            goto lossage;

          v->label = xstrdup (label);
          asciify (v->label);
        }
    }
  ext->case_size = ext->dict->nval;

  if (ext->dict->weight_var[0] != 0
      && !find_dict_variable (ext->dict, ext->dict->weight_var))
    lose ((_("Weighting variable %s not present in dictionary"),
           ext->dict->weight_var));

  return 1;

 lossage:
  return 0;



    }

        /* Creates a variable named NAME in dictionary DICT having type TYPE
   (ALPHA or NUMERIC) and, if type==ALPHA, width WIDTH.  Returns a
   pointer to the newly created variable if successful.  On failure
   (which indicates that a variable having the specified name already
   exists), returns NULL.  */
static variable create_variable (dictionary dict, string name,  VariableType type, int width)
{
  if (find_dict_variable (dict, name))
    return null;

  {
    variable new_var;

    new_var = new variable();
    dict.var.Add(new_var);

    new_var.index = dict.nvar;
    dict.nvar++;

    init_variable (dict, new_var, name, type, width);

    return new_var;
  }
}

        /* Initialize (for the first time) a variable V in dictionary DICT
   with name NAME, type TYPE, and width WIDTH.  */
void
init_variable (dictionary dict, variable v, string name,
               VariableType type, int width)
{
  common_init_stuff (dict, v, name, type, width);
  v.nv = type == NUMERIC ? 1 : DIV_RND_UP (width, 8);
  v.fv = dict.nval;
  dict.nval += v.nv;
  v.label = null;
  v.val_lab = null;
  v.get.fv = -1;
}
        /* Initialize fields in variable V inside dictionary D with name NAME,
   type TYPE, and width WIDTH.  Initializes some other fields too. */
static void common_init_stuff (dictionary dict, variable v,
                   string name, int type, int width)
{
  if (v.name != name)
    /* Avoid problems with overlap. */
    strcpy (v.name, name);

  avl_force_insert (dict.var_by_name, v);

  v.type = type;
  v.left = name[0] == '#';
  v.width = type == NUMERIC ? 0 : width;
  v.miss_type = MISSING_NONE;
  if (v.type == NUMERIC)
    {
      v.print.type = FMT_F;
      v.print.w = 8;
      v.print.d = 2;
    }
  else
    {
      v.print.type = FMT_A;
      v.print.w = v.width;
      v.print.d = 0;
    }
  v.write = v.print;
}

        static int convert_format (FileStream h, int[] fmt, fmt_spec v, variable vv)
{
   //  if (fmt[0] < 0 || (size_t) fmt[0] >= sizeof(translate_fmt) / sizeof(translate_fmt))
   // throw new ApplicationException(string.Format("%s: Bad format specifier byte %d", vv->name, fmt[0]));

  v.type = translate_fmt[fmt[0]];
  v.w = fmt[1];
  v.d = fmt[2];

  /* FIXME?  Should verify the resulting specifier more thoroughly. */

  if (v.type == -1)
    throw new ApplicationException(string.Format("%s: Bad format specifier byte (%d)", vv.name, fmt[0]));
  if ((vv.type == VariableType.ALPHA) ^ ((formats[v.type].cat & FCAT_STRING) != 0))
    throw new ApplicationException(string.Format("%s variable %s has %s format specifier %s", vv.type == VariableType.ALPHA ? "String" : "Numeric",
           vv.name,
           formats[v.type].cat & FCAT_STRING ? "string" : "numeric",
           formats[v.type].name));
  return 1;

 lossage:
  return 0;
}
        
        
        /* Parse a value for variable VV into value V.  Returns success. */
static bool parse_value (FileStream h, object v, variable vv)
{
  if (vv.type == VariableType.ALPHA)
    {
      string mv = read_string(h);
      int j;

      if (mv == null)
        return false;

      v.s = mv;
      for (j = 0; j < 8; j++)
        if (v.s[j])
          v.s[j] = spss2ascii[v.s[j]];
        else
          /* Value labels are always padded with spaces. */
          v.s[j] = ' ';
    }
  else
    {
      v->f = read_float (h);
      if (v.f == SECOND_LOWEST_VALUE)
        return false;
    }

  return true;
}




        /* Information produced by pfm_read_dictionary() that doesn't fit into
           a dictionary struct. */
        struct pfm_read_info
        {
            public char[] creation_date;     /* `dd mm yyyy' plus a null. */
            public char[] creation_time;      /* `hh:mm:ss' plus a null. */
            public char[] product;           /* Product name plus a null. */
            public char[] subproduct;        /* Subproduct name plus a null. */
        }

        /* pfm's file_handle extension. */
        struct pfm_fhuser_ext
        {
            public FileStream file;                 /* Actual file. */

            public dictionary dict;    /* File's dictionary. */
            public int weight_index;           /* 0-based index of weight variable, or -1. */

            public Dictionary<byte, byte> trans;       /* 256-byte character set translation table. */

            public int nvars;                  /* Number of variables. */
            public int[] vars;                  /* Variable widths, 0 for numeric. */
            public int case_size;              /* Number of `value's per case. */

            public byte[] buf;      /* Input buffer. */
            //    public byte bp;          /* Buffer pointer. */
            public int cc;                     /* Current character. */
        };

        /* Display format. */
        struct fmt_spec
        {
            public int type;                   /* One of the above constants. */
            public int w;                      /* Width. */
            public int d;                      /* Number of implied decimal places. */
        };


        /* A variable's dictionary entry.  Note: don't reorder name[] from the
           first element; a pointer to `variable' should be a pointer to
           member `name'.*/
        struct variable
        {
            /* Required by parse_variables() to be in this order.  */
            public char[] name;               /* As a string (9 chars). */
            public int index;                  /* Index into its dictionary's var[]. */
            public VariableType type;                   /* NUMERIC or ALPHA. */
            public int foo;                    /* Used for temporary storage. */

            /* Also important but parse_variables() doesn't need it.  Still,
               check before reordering. */
            public int width;                  /* Size of string variables in chars. */
            public int fv, nv;                 /* Index into `value's, number of values. */
            public int left;                   /* 0=do not LEAVE, 1=LEAVE. */

            /* Missing values. */
            public int miss_type;              /* One of the MISSING_* constants. */
            public object[] missing;     /* User-missing value. */

            /* Display formats. */
            public fmt_spec print;      /* Default format for PRINT. */
            public fmt_spec write;      /* Default format for WRITE. */

            /* Labels. */
            public object val_lab;   /* Avltree of value_label structures. */
            public string label;                /* Variable label. */

            /* Per-procedure info. */
            //get_proc get;

            public procedureinfo p;

            struct procedureinfo
            {
                //crosstab_proc crs;
                //descriptives_proc dsc;
                //frequencies_proc frq;
                //list_proc lst;
                //means_proc mns;
                //sort_cases_proc srt;
                //modify_vars_proc mfv;
                //matrix_data_proc mxd;
                //match_files_proc mtf;
            }
        }

        /* Variable type. */
        enum VariableType
        {
            NUMERIC,                    /* A numeric variable. */
            ALPHA                       /* A string variable.  (STRING is pre-empted by lexer.h) */
        }


        /* Complete dictionary state. */
        class dictionary
  {
      public List<variable> var;      /* Variable descriptions. */
      public List<variable> var_by_name;       /* Variables arranged by name. */
      public int nvar;                   /* Number of variables. */

      public int N;                      /* Current case limit (N command). */
      public int nval;                   /* Number of value structures per case. */

      public int n_splits;               /* Number of SPLIT FILE variables. */
      public List<variable> splits;   /* List of SPLIT FILE vars. */

      public string label;                /* File label. */

      public int n_documents;            /* Number of lines of documents. */
      public string[] documents;            /* Documents; 80*n_documents bytes in size. */

      public int weight_index;           /* `value' index of $WEIGHT, or -1 if none. Call update_weighting() before using! */
      public string weight_var;         /* Name of WEIGHT variable. */

      public string filter_var;         /* Name of FILTER variable. */
    /* Do not make another field the last field! or see
       temporary.c:restore_dictionary() before doing so! */
  }

        /* Types of missing values.  Order is significant, see
   mis-val.c:parse_numeric(), sfm-read.c:sfm_read_dictionary()
   sfm-write.c:sfm_write_dictionary(),
   sysfile-info.c:cmd_sysfile_info(), mis-val.c:copy_missing_values(),
   pfm-read.c:read_variables(), pfm-write.c:write_variables(),
   apply-dict.c:cmd_apply_dictionary(), and more (?). */
enum MissingTypes
  {
    MISSING_NONE,               /* No user-missing values. */
    MISSING_1,                  /* One user-missing value. */
    MISSING_2,                  /* Two user-missing values. */
    MISSING_3,                  /* Three user-missing values. */
    MISSING_RANGE,              /* [a,b]. */
    MISSING_LOW,                /* (-inf,a]. */
    MISSING_HIGH,               /* (a,+inf]. */
    MISSING_RANGE_1,            /* [a,b], c. */
    MISSING_LOW_1,              /* (-inf,a], b. */
    MISSING_HIGH_1,             /* (a,+inf), b. */
    MISSING_COUNT
  }

  /* PORTME: Set the value for second_lowest_value, which is the
     "second lowest" possible value for a double.  This is the value
     for LOWEST on MISSING VALUES, etc. */
const double SECOND_LOWEST_VALUE = {{0xff, 0xef, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfe}};


const fmt_r[] translate_fmt =
  {
    -1, FMT_A, FMT_AHEX, FMT_COMMA, FMT_DOLLAR, FMT_F, FMT_IB, FMT_PIBHEX, FMT_P, FMT_PIB, FMT_PK, FMT_RB, FMT_RBHEX, -1, -1, FMT_Z, FMT_N, FMT_E, -1, -1, FMT_DATE, FMT_TIME, FMT_DATETIME, FMT_ADATE, FMT_JDATE, FMT_DTIME, FMT_WKDAY, FMT_MONTH, FMT_MOYR, FMT_QYR, FMT_WKYR, FMT_PCT, FMT_DOT, FMT_CCA, FMT_CCB, FMT_CCC, FMT_CCD, FMT_CCE, FMT_EDATE, FMT_SDATE
  }

        class fmt_r
        {
            public string Name;
            public string Name2;
            public int int1;
            public int int2;
            public int int3;
            public int int4;
            public int int5;
            public int int6;
            public string Name3;
            public int int7;
            
            public fmt_r(string Name, string Name2, int int1, int int2, int int3, int int4, int int5, int int6, string Name3, int int7)
            {
                this.Name = Name;
                this.Name2 = Name2;
                this.Name3 = Name3;
                this.int1 = int1;
                this.int2 = int2;
                this.int3 = int3;
                this.int4 = int4;
                this.int5 = int5;
                this.int6 = int6;
                this.int7 = int7;
            }
            
            static Dictionary<string, fmt_r> GetDictionary()
            {
               var d = new Dictionary<sring,fmt_r>();
                d.Add("FMT_F", new fmt_r("FMT_F",            "F",         2,  1,  40,  1,   40, 0001, "FMT_F", 5));
                d.Add("FMT_N", new fmt_r("FMT_N",            "N",         2,  1,  40,  1,   40, 0011, "FMT_F", 16));
                d.Add("FMT_E", new fmt_r("FMT_E",            "E",         2,  1,  40,  6,   40, 0001, "FMT_E", 17));
                d.Add("FMT_COMMA", new fmt_r("FMT_COMMA",        "COMMA",     2,  1,  40,  1,   40, 0001, "FMT_COMMA", 3));
                d.Add("FMT_DOT", new fmt_r("FMT_DOT",          "DOT",       2,  1,  40,  1,   40, 0001, "FMT_DOT", 32));
                d.Add("FMT_DOLLAR", new fmt_r("FMT_DOLLAR",       "DOLLAR",    2,  1,  40,  2,   40, 0001, "FMT_DOLLAR", 4));
                d.Add("FMT_PCT", new fmt_r("FMT_PCT",          "PCT",       2,  1,  40,  2,   40, 0001, "FMT_PCT", 31));
                d.Add("FMT_Z", new fmt_r("FMT_Z",            "Z",         2,  1,  40,  1,   40, 0011, "FMT_F", 15));
                d.Add("FMT_A", new fmt_r("FMT_A",            "A",         1,  1, 255,  1,  254, 0004, "FMT_A", 1));
                d.Add(1, new fmt_r("FMT_AHEX",         "AHEX",      1,  2, 254,  2,  510, 0006, "FMT_A", 2));
                d.Add(1, new fmt_r("FMT_IB",           "IB",        2,  1,   8,  1,    8, 0010, "FMT_F", 6));
                d.Add("FMT_P", new fmt_r("FMT_P",            "P",         2,  1,  16,  1,   16, 0011, "FMT_F", 8));
                d.Add("FMT_PIB", new fmt_r("FMT_PIB",          "PIB",       2,  1,   8,  1,    8, 0010, "FMT_F", 9));
                d.Add("FMT_PIBHEX", new fmt_r("FMT_PIBHEX",       "PIBHEX",    2,  2,  16,  2,   16, 0002, "FMT_F", 7));
                d.Add("FMT_PK", new fmt_r("FMT_PK",           "PK",        2,  1,  16,  1,   16, 0010, "FMT_F", 10));
                d.Add("FMT_RB", new fmt_r("FMT_RB",           "RB",        1,  2,   8,  2,    8, 0002, "FMT_F", 11));
                d.Add("FMT_RBHEX", new fmt_r("FMT_RBHEX",        "RBHEX",     1,  4,  16,  4,   16, 0002, "FMT_F", 12));

                /* Custom currency. */
                d.Add("FMT_CCA", new fmt_r("FMT_CCA",          "CCA",       2, -1,  -1,  1,   40, 0020, "FMT_CCA", 33));
                d.Add("FMT_CCB", new fmt_r("FMT_CCB",          "CCB",       2, -1,  -1,  1,   40, 0020, "FMT_CCB", 34));
                d.Add("FMT_CCC", new fmt_r("FMT_CCC",          "CCC",       2, -1,  -1,  1,   40, 0020, "FMT_CCC", 35));
                d.Add("FMT_CCD", new fmt_r("FMT_CCD",          "CCD",       2, -1,  -1,  1,   40, 0020, "FMT_CCD", 36));
                d.Add("FMT_CCE", new fmt_r("FMT_CCE",          "CCE",       2, -1,  -1,  1,   40, 0020, "FMT_CCE", 37));
                                    
                /* Date/time formats. */
                d.Add("FMT_DATE", new fmt_r("FMT_DATE",         "DATE",      1,  9,  40,  9,   40, 0001, "FMT_DATE", 20));
                d.Add("FMT_EDATE", new fmt_r("FMT_EDATE",        "EDATE",     1,  8,  40,  8,   40, 0001, "FMT_EDATE", 23));
                d.Add("FMT_SDATE", new fmt_r("FMT_SDATE",        "SDATE",     1,  8,  40,  8,   40, 0001, "FMT_SDATE", 24));
                d.Add("FMT_ADATE", new fmt_r("FMT_ADATE",        "ADATE",     1,  8,  40,  8,   40, 0001, "FMT_ADATE", 29));
                d.Add("FMT_JDATE", new fmt_r("FMT_JDATE",        "JDATE",     1,  5,  40,  5,   40, 0001, "FMT_JDATE", 28));
                d.Add("FMT_QYR", new fmt_r("FMT_QYR",          "QYR",       1,  4,  40,  6,   40, 0001, "FMT_QYR", 30));
                d.Add("FMT_MOYR", new fmt_r("FMT_MOYR",         "MOYR",      1,  6,  40,  6,   40, 0001, "FMT_MOYR", 22));
                d.Add("FMT_WKYR", new fmt_r("FMT_WKYR",         "WKYR",      1,  6,  40,  8,   40, 0001, "FMT_WKYR", 21));
                d.Add("FMT_DATETIME", new fmt_r("FMT_DATETIME",     "DATETIME",  2, 17,  40, 17,   40, 0001, "FMT_DATETIME", 38));
                d.Add("FMT_TIME", new fmt_r("FMT_TIME",         "TIME",      2,  5,  40,  5,   40, 0001, "FMT_TIME", 39));
                d.Add("FMT_DTIME", new fmt_r("FMT_DTIME",        "DTIME",     2, 11,  40,  8,   40, 0001, "FMT_DTIME", 25));
                d.Add("FMT_WKDAY", new fmt_r("FMT_WKDAY",        "WKDAY",     1,  2,  40,  2,   40, 0001, "FMT_WKDAY", 26));
                d.Add("FMT_MONTH", new fmt_r("FMT_MONTH",        "MONTH",     1,  3,  40,  3,   40, 0001, "FMT_MONTH", 27));

                /* These aren't real formats.  They're used by DATA LIST. */
                d.Add("FMT_T", new fmt_r("FMT_T",            "T",         1,  1,99999, 1,99999, 0000, "FMT_T", -1));
                d.Add("FMT_X", new fmt_r("FMT_X",            "X",         1,  1,99999, 1,99999, 0000, "FMT_X", -1));
                d.Add("FMT_DESCEND", new fmt_r("FMT_DESCEND",      "***",       1,  1,99999, 1,99999, 0000, -1, -1));
                d.Add("FMT_NEWREC", new fmt_r("FMT_NEWREC",       "***",       1,  1,99999, 1,99999, 0000, -1, -1));
            }

            static fmt_r translate_fmt(int i)
            {
                var d = GetDictionary();
                switch (i)
                {
                    case 0: return -1;
                    case 1: return d["FMT_A"];
                    case 2: return d["FMT_AHEX"];
                    case 3: return d["FMT_COMMA"];
                    case 4: return d["FMT_DOLLAR"];
                    case 5:  return d["FMT_F"];
                    case 6: return d["FMT_IB"];
                    case 7: return d["FMT_PIBHEX"];
                    case 8: return d["FMT_P"];
                    case 9: return d["FMT_PIB"];
                    case 10: return d["FMT_PK"];
                    case 11: return d["FMT_RB"];
                    case 12: return d["FMT_RBHEX"];
                    case 13: return -1;
                    case 14: return -1;
                    case 15: return d["FMT_Z"];
                    case 16: return d["FMT_N"];
                     case 17: return d["FMT_E"];
                     case 18: return d-1;
                     case 19: return d-1;
                     case 20: return d["FMT_DATE"];
                     case 21: return d["FMT_TIME"];
                     case 22: return d["FMT_DATETIME"];
                     case 23: return d["FMT_ADATE"];
                     case 24: return d["FMT_JDATE"];
                     case 25: return d["FMT_DTIME"];
                     case 26: return d["FMT_WKDAY"];
                     case 27: return d["FMT_MONTH"];
                     case 28: return d["FMT_MOYR"];
                     case 29: return d["FMT_QYR"];
                     case 30: return d["FMT_WKYR"]; 
                     case 31: return d["FMT_PCT"];
                     case 32: return d["FMT_DOT"]; 
                     case 33: return d["FMT_CCA"];
                     case 34: return d["FMT_CCB"]; 
                     case 35: return d["FMT_CCC"]; 
                     case 36: return d["FMT_CCD"]; 
                     case 37: return d["FMT_CCE"]; 
                     case 38: return d["FMT_EDATE"]; 
                     case 39: return d["FMT_SDATE"];
                }
            }
                
        }
    }
}
