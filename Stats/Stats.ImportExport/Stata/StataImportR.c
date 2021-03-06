/**
  Read  Stata version 8.0, 7.0, 7/SE, 6.0 and 5.0 .dta files, write version 7.0, 6.0.

  (c) 1999, 2000, 2001, 2002 Thomas Lumley.
  2000 Saikat DebRoy

  The format of Stata files is documented under 'file formats'
  in the Stata manual.

  This code currently does not make use of the print format information in
   a .dta file (except for dates). It cannot handle files with 'int'
  'float' or 'double' that differ from IEEE 4-byte integer, 4-byte
  real and 8-byte real respectively: it's not clear whether such files
  can exist.

  Versions of Stata before 4.0 used different file formats.

**/


#include <stdio.h>
#include "foreign.h"
#include "swap_bytes.h"

/* versions */
#define VERSION_5 0x69
#define VERSION_6 'l'
#define VERSION_7 0x6e
#define VERSION_7SE 111
#define VERSION_8 113

/* Stata format constants */
#define STATA_FLOAT  'f'
#define STATA_DOUBLE 'd'
#define STATA_INT    'l'
#define STATA_SHORTINT 'i'
#define STATA_BYTE  'b'

#define STATA_SE_STRINGOFFSET 0
#define STATA_SE_FLOAT  254
#define STATA_SE_DOUBLE 255
#define STATA_SE_INT    253
#define STATA_SE_SHORTINT 252
#define STATA_SE_BYTE  251

#define STATA_STRINGOFFSET 0x7f

#define STATA_BYTE_NA 127
#define STATA_SHORTINT_NA 32767
#define STATA_INT_NA 2147483647

#define STATA_FLOAT_NA pow(2.0, 127)
#define STATA_DOUBLE_NA pow(2.0, 1023)

static int stata_endian;


/** Low-level input **/

static int InIntegerBinary(FILE * fp, int naok, int swapends)
{
    int i;
    if (fread(&i, sizeof(int), 1, fp) != 1)
        error(_("a binary read error occurred"));
    if (swapends)
        reverse_int(i);
    return ((i==STATA_INT_NA) & !naok ? NA_INTEGER : i);
}
/* read a 1-byte signed integer */
static int InByteBinary(FILE * fp, int naok)
{
    signed char i;
    if (fread(&i, sizeof(char), 1, fp) != 1)
        error(_("a binary read error occurred"));
    return  ((i==STATA_BYTE_NA) & !naok ? NA_INTEGER : (int) i);
}
/* read a single byte  */
static int RawByteBinary(FILE * fp, int naok)
{
    unsigned char i;
    if (fread(&i, sizeof(char), 1, fp) != 1)
        error(_("a binary read error occurred"));
    return  ((i==STATA_BYTE_NA) & !naok ? NA_INTEGER : (int) i);
}

static int InShortIntBinary(FILE * fp, int naok,int swapends)
{
        unsigned first,second;
        int result;

  first = RawByteBinary(fp,1);
  second = RawByteBinary(fp,1);
  if (stata_endian == CN_TYPE_BIG){
    result= (first<<8) | second;
  } else {
    result= (second<<8) | first;
  }
  if (result>STATA_SHORTINT_NA) result-=65536;
  return ((result==STATA_SHORTINT_NA) & !naok ? NA_INTEGER  : result);
}


static double InDoubleBinary(FILE * fp, int naok, int swapends)
{
    double i;
    if (fread(&i, sizeof(double), 1, fp) != 1)
        error(_("a binary read error occurred"));
    if (swapends)
        reverse_double(i);
    return ((i==STATA_DOUBLE_NA) & !naok ? NA_REAL : i);
}

static double InFloatBinary(FILE * fp, int naok, int swapends)
{
    float i;
    if (fread(&i, sizeof(float), 1, fp) != 1)
        error(_("a binary read error occurred"));
    if (swapends)
        reverse_float(i);
    return ((i==STATA_FLOAT_NA) & !naok ? NA_REAL :  (double) i);
}

static void InStringBinary(FILE * fp, int nchar, char* buffer)
{
    if (fread(buffer, nchar, 1, fp) != 1)
        error(_("a binary read error occurred"));
}

/** now optional and done at R level **/
static char* nameMangle(char *stataname, int len){
  return stataname;
}
/**static char* nameMangle(char *stataname, int len){
    int i;
    for(i=0;i<len;i++)
      if (stataname[i]=='_') stataname[i]='.';
    return stataname;
}
**/

/*****
      Turn a .dta file into a data frame
      Variable labels go to attributes of the data frame, value labels go to factor levels

     characteristics could go as attributes of the variables
      not yet implemented
****/



SEXP R_LoadStataData(FILE *fp)
{
    int i,j,nvar,nobs,charlen, version,swapends,varnamelength,nlabels,totlen;
    unsigned char abyte;
    char datalabel[81], timestamp[18], aname[33];
    SEXP df,names,tmp,tmp1,varlabels,types,row_names;
    SEXP levels, labels,labeltable, sversion;
    char stringbuffer[129], *txt;
    int *off;


    /** first read the header **/

    abyte=RawByteBinary(fp,1);   /* release version */
    version=0;                  /* -Wall */
    varnamelength=0;            /* -Wall */
    labeltable = R_NilValue;    /* -Wall */
    switch (abyte){
    case VERSION_5:
        version=5;
        varnamelength=8;
        break;
    case VERSION_6:
        version=6;
        varnamelength=8;
        break;
    case VERSION_7:
        version=7;
        varnamelength=32;
        break;
    case VERSION_7SE:
        version=-7;
        varnamelength=32;
        break;
    case VERSION_8:
        version=-8;  /* version 8 automatically uses SE format */
        varnamelength=32;
        break;
    default:
        error(_("not a Stata version 5-8 .dta file"));
    }
    stata_endian=(int) RawByteBinary(fp,1);     /* byte ordering */
    swapends = stata_endian != CN_TYPE_NATIVE;

    RawByteBinary(fp,1);            /* filetype -- junk */
    RawByteBinary(fp,1);            /* padding */
    nvar =  (InShortIntBinary(fp,1,swapends)); /* number of variables */
    nobs =(InIntegerBinary(fp,1,swapends));  /* number of cases */
    /* data label - zero terminated string */
    switch (abs(version)) {
    case 5:
        InStringBinary(fp,32,datalabel);
        break;
    case 6:
    case 7:
    case 8:
        InStringBinary(fp,81,datalabel);
        break;
    }
    /* file creation time - zero terminated string */
    InStringBinary(fp,18,timestamp);

    /** make the data frame **/

    PROTECT(df=allocVector(VECSXP, nvar));

    /** and now stick the labels on it **/

    PROTECT(tmp=allocVector(STRSXP,1));
    SET_STRING_ELT(tmp,0,mkChar(datalabel));
    setAttrib(df,install("datalabel"),tmp);
    UNPROTECT(1);

    PROTECT(tmp=allocVector(STRSXP,1));
    SET_STRING_ELT(tmp,0,mkChar(timestamp));
    setAttrib(df,install("time.stamp"),tmp);
    UNPROTECT(1);


    /** read variable descriptors **/

    /** types **/

    PROTECT(types=allocVector(INTSXP,nvar));
    if (version>0){
            for(i=0;i<nvar;i++){
                    abyte = RawByteBinary(fp,1);
                    INTEGER(types)[i]= abyte;
                    switch (abyte) {
                    case STATA_FLOAT:
                    case STATA_DOUBLE:
                            SET_VECTOR_ELT(df,i,allocVector(REALSXP,nobs));
                            break;
                    case STATA_INT:
                    case STATA_SHORTINT:
                    case STATA_BYTE:
                            SET_VECTOR_ELT(df,i,allocVector(INTSXP,nobs));
                            break;
                    default:
                            if (abyte<STATA_STRINGOFFSET)
                                    error(_("unknown data type"));
                            SET_VECTOR_ELT(df,i,allocVector(STRSXP,nobs));
                            break;
                    }
            }
    } else {
            for(i=0;i<nvar;i++){
                    abyte = RawByteBinary(fp,1);
                    INTEGER(types)[i]= abyte;
                    switch (abyte) {
                    case STATA_SE_FLOAT:
                    case STATA_SE_DOUBLE:
                            SET_VECTOR_ELT(df,i,allocVector(REALSXP,nobs));
                            break;
                    case STATA_SE_INT:
                    case STATA_SE_SHORTINT:
                    case STATA_SE_BYTE:
                            SET_VECTOR_ELT(df,i,allocVector(INTSXP,nobs));
                            break;
                    default:
                            if (abyte>244)
                                    error(_("unknown data type"));
                            SET_VECTOR_ELT(df,i,allocVector(STRSXP,nobs));
                            break;
                    }
            }
    }

    /** names **/

    PROTECT(names=allocVector(STRSXP,nvar));
    for (i=0;i<nvar;i++){
        InStringBinary(fp,varnamelength+1,aname);
        SET_STRING_ELT(names,i,mkChar(nameMangle(aname,varnamelength+1)));
    }
    setAttrib(df,R_NamesSymbol, names);

    UNPROTECT(1);

    /** sortlist -- not relevant **/

    for (i=0;i<2*(nvar+1);i++)
        RawByteBinary(fp,1);

    /** format list
        passed back to R as attributes.
        Used to identify date variables.
    **/

    PROTECT(tmp=allocVector(STRSXP,nvar));
    for (i=0;i<nvar;i++){
        InStringBinary(fp,12,timestamp);
        SET_STRING_ELT(tmp,i,mkChar(timestamp));
    }
    setAttrib(df,install("formats"),tmp);
    UNPROTECT(1);
    setAttrib(df,install("types"),types);


    /** value labels.  These are stored as the names of label formats,
        which are themselves stored later in the file. **/

    PROTECT(tmp=allocVector(STRSXP,nvar));
    for(i=0;i<nvar;i++){
        InStringBinary(fp,varnamelength+1,aname);
        PROTECT(tmp1=allocString(strlen(aname)));
        strcpy(CHAR(tmp1),aname);
        SET_STRING_ELT(tmp,i,tmp1);
        UNPROTECT(1);
    }
    setAttrib(df,install("val.labels"),tmp);
    UNPROTECT(1); /*tmp*/

    /** Variable Labels **/

    PROTECT(varlabels=allocVector(STRSXP,nvar));

    switch(abs(version)){
    case 5:
        for(i=0;i<nvar;i++) {
            InStringBinary(fp,32,datalabel);
            SET_STRING_ELT(varlabels,i,mkChar(datalabel));
        }
        break;
    case 6:
    case 7:
    case 8:
        for(i=0;i<nvar;i++) {
            InStringBinary(fp,81,datalabel);
            SET_STRING_ELT(varlabels,i,mkChar(datalabel));
        }
    }
    setAttrib(df, install("var.labels"), varlabels);

    UNPROTECT(1);

    /** variable 'characteristics'  -- not yet implemented **/

    while(RawByteBinary(fp,1)) {
        if (abs(version)>=7) /* manual is wrong here */
            charlen= (InIntegerBinary(fp,1,swapends));
        else
            charlen= (InShortIntBinary(fp,1,swapends));
        for (i=0;i<charlen;i++)
          InByteBinary(fp,1);
    }
    if (abs(version)>=7)
        charlen= (InIntegerBinary(fp,1,swapends));
    else
        charlen=(InShortIntBinary(fp,1,swapends));
    if (charlen!=0)
      error(_("something strange in the file\n (Type 0 characteristic of nonzero length)"));


    /** The Data **/

    if (version>0) { /* not Stata/SE */
            for(i=0;i<nobs;i++){
                    for(j=0;j<nvar;j++){
                            switch (INTEGER(types)[j]) {
                            case STATA_FLOAT:
                                    REAL(VECTOR_ELT(df,j))[i]=(InFloatBinary(fp,0,swapends));
                                    break;
                            case STATA_DOUBLE:
                                    REAL(VECTOR_ELT(df,j))[i]=(InDoubleBinary(fp,0,swapends));
                                    break;
                            case STATA_INT:
                                    INTEGER(VECTOR_ELT(df,j))[i]=(InIntegerBinary(fp,0,swapends));
                                    break;
                            case STATA_SHORTINT:
                                    INTEGER(VECTOR_ELT(df,j))[i]=(InShortIntBinary(fp,0,swapends));
                                    break;
                            case STATA_BYTE:
                                    INTEGER(VECTOR_ELT(df,j))[i]=(int) InByteBinary(fp,0);
                                    break;
                            default:
                                    charlen=INTEGER(types)[j]-STATA_STRINGOFFSET;
                                    InStringBinary(fp,charlen,stringbuffer);
                                    stringbuffer[charlen]=0;
                                    PROTECT(tmp=allocString(strlen(stringbuffer)));
                                    strcpy(CHAR(tmp),stringbuffer); /*we know strcpy is safe here*/
                                    SET_STRING_ELT(VECTOR_ELT(df,j),i,tmp);
                                    UNPROTECT(1);
                                    break;
                            }
                    }
            }
    }  else {
            for(i=0; i<nobs; i++){
                    for(j=0;j<nvar;j++){
                            switch (INTEGER(types)[j]) {
                            case STATA_SE_FLOAT:
                                    REAL(VECTOR_ELT(df,j))[i]=(InFloatBinary(fp,0,swapends));
                                    break;
                            case STATA_SE_DOUBLE:
                                    REAL(VECTOR_ELT(df,j))[i]=(InDoubleBinary(fp,0,swapends));
                                    break;
                            case STATA_SE_INT:
                                    INTEGER(VECTOR_ELT(df,j))[i]=(InIntegerBinary(fp,0,swapends));
                                    break;
                            case STATA_SE_SHORTINT:
                                    INTEGER(VECTOR_ELT(df,j))[i]=(InShortIntBinary(fp,0,swapends));
                                    break;
                            case STATA_SE_BYTE:
                                    INTEGER(VECTOR_ELT(df,j))[i]=(int) InByteBinary(fp,0);
                                    break;
                            default:
                                    charlen=INTEGER(types)[j]-STATA_SE_STRINGOFFSET;
                                    InStringBinary(fp,charlen,stringbuffer);
                                    stringbuffer[charlen]=0;
                                    PROTECT(tmp=allocString(strlen(stringbuffer)));
                                    strcpy(CHAR(tmp),stringbuffer); /*we know strcpy is safe here*/
                                    SET_STRING_ELT(VECTOR_ELT(df,j),i,tmp);
                                    UNPROTECT(1);
                                    break;
                            }
                    }
            }
    }


    /** value labels **/
    if (abs(version)>5){
            PROTECT(labeltable=allocVector(VECSXP, nvar));
            PROTECT(tmp=allocVector(STRSXP,nvar));
            for(j=0;j<nvar;j++){
                    /* first int not needed, use fread directly to trigger EOF */
                    fread((int *) aname,sizeof(int),1,fp);
                    if (feof(fp))
                            break;
                    InStringBinary(fp,varnamelength+1,aname);
                    SET_STRING_ELT(tmp,j,mkChar(aname));
                    RawByteBinary(fp,1);RawByteBinary(fp,1);RawByteBinary(fp,1); /*padding*/
                    nlabels=InIntegerBinary(fp,1,swapends);
                    totlen=InIntegerBinary(fp,1,swapends);
                    off= Calloc((size_t) nlabels, int);
                    PROTECT(levels=allocVector(REALSXP,nlabels));
                    PROTECT(labels=allocVector(STRSXP,nlabels));
                    for(i=0;i<nlabels;i++)
                            off[i]=InIntegerBinary(fp,1,swapends);
                    for(i=0;i<nlabels;i++)
                            REAL(levels)[i]=(double) InIntegerBinary(fp,0,swapends);
                    txt= Calloc((size_t) totlen, char);
                    InStringBinary(fp,totlen,txt);
                    for(i=0;i<nlabels;i++){
                            SET_STRING_ELT(labels,i,mkChar(txt+off[i]));
                    }
                    namesgets(levels,labels);
                    SET_VECTOR_ELT(labeltable,j,levels);
                    Free(off);
                    Free(txt);
                    UNPROTECT(2);/* levels, labels */
            }
            namesgets(labeltable,tmp);
            UNPROTECT(1); /*tmp*/
    }

    /** tidy up **/

    PROTECT(row_names = allocVector(STRSXP, nobs));
    for (i=0; i<nobs; i++) {
        sprintf(datalabel, "%d", i+1);
        SET_STRING_ELT(row_names,i,mkChar(datalabel));
    }
    setAttrib(df, R_RowNamesSymbol, row_names);
    UNPROTECT(1);

    PROTECT(sversion=allocVector(INTSXP,1));
    INTEGER(sversion)[0]=version;
    setAttrib(df, install("version"), sversion);
    UNPROTECT(1);

    if (abs(version)>5){
            setAttrib(df, install("label.table"), labeltable);
            UNPROTECT(1); /*labeltable*/;
    }
    UNPROTECT(2); /* types, df */

    return(df);

}
SEXP do_readStata(SEXP call)
{
    SEXP fname,  result;
    FILE *fp;

    if ((sizeof(double)!=8) | (sizeof(int)!=4) | (sizeof(float)!=4))
      error(_("can not yet read Stata .dta on this platform"));


    if (!isValidString(fname = CADR(call)))
        error(_("first argument must be a file name\n"));

    fp = fopen(R_ExpandFileName(CHAR(STRING_ELT(fname,0))), "rb");
    if (!fp)
        error(_("unable to open file"));

    result = R_LoadStataData(fp);
    fclose(fp);
    return result;
}


/** low level output **/

static void OutIntegerBinary(int i, FILE * fp, int naok)
{
    i=((i==NA_INTEGER) & !naok ? STATA_INT_NA : i);
    if (fwrite(&i, sizeof(int), 1, fp) != 1)
        error(_("a binary write error occurred"));

}

static void OutByteBinary(unsigned char i, FILE * fp)
{
    if (fwrite(&i, sizeof(char), 1, fp) != 1)
        error(_("a binary write error occurred"));
}
static void OutDataByteBinary(int i, FILE * fp)
{
    i=(unsigned char) ((i==NA_INTEGER) ? STATA_BYTE_NA : i);
    if (fwrite(&i, sizeof(char), 1, fp) != 1)
        error(_("a binary write error occurred"));
}

static void OutShortIntBinary(int i,FILE * fp)
{
  unsigned char first,second;

#ifdef WORDS_BIGENDIAN
    first= (i>>8);
    second=i & 0xff;
#else
    first=i & 0xff;
    second=i>>8;
#endif
  if (fwrite(&first, sizeof(char), 1, fp) != 1)
    error(_("a binary write error occurred"));
  if (fwrite(&second, sizeof(char), 1, fp) != 1)
    error(_("a binary write error occurred"));
}


static void  OutDoubleBinary(double d, FILE * fp, int naok)
{
    d=(R_FINITE(d) ? d : STATA_DOUBLE_NA);
    if (fwrite(&d, sizeof(double), 1, fp) != 1)
        error(_("a binary write error occurred"));
}


static void OutStringBinary(char *buffer, FILE * fp, int nchar)
{
    if (nchar==0) return;
    if (fwrite(buffer, nchar, 1, fp) != 1)
        error(_("a binary write error occurred"));
}

static char* nameMangleOut(char *stataname, int len){
    int i;
    for(i=0;i<len;i++){
      if (stataname[i]=='.') stataname[i]='_';
    }
    return stataname;
}

void R_SaveStataData(FILE *fp, SEXP df, int version, SEXP leveltable)
{
    int i,j,k,l,nvar,nobs,charlen,txtlen,len;
    char datalabel[81]="Written by R.              ", timestamp[18], aname[33];
    char format9g[12]="%9.0g", strformat[12]="";
    SEXP names,types,theselabels,orig_names;

    int namelength=8;
    if (version==7 || version==8)
        namelength=32;
    k=0; /* -Wall */

    /* names are 32 characters in version 7 */

    /** first write the header **/
    if (version==6)
        OutByteBinary((char) VERSION_6,fp);            /* release */
    else if (version==7)
        OutByteBinary((char) VERSION_7,fp);
    else if (version==8)
        OutByteBinary((char) VERSION_8,fp);
    OutByteBinary((char) CN_TYPE_NATIVE, fp);
    OutByteBinary(1,fp);            /* filetype */
    OutByteBinary(0,fp);            /* padding */

    nvar=length(df);
    OutShortIntBinary(nvar,fp);
    nobs=length(VECTOR_ELT(df,0));
    OutIntegerBinary(nobs,fp,1);  /* number of cases */
    OutStringBinary(datalabel,fp,81);   /* data label - zero terminated string */
    for(i=0;i<18;i++){
      timestamp[i]=0;
    }
    OutStringBinary(timestamp,fp,18);   /* file creation time - zero terminated string */



    /** write variable descriptors **/

    /** types **/
    /* FIXME: writes everything as double or integer to save effort*/
    /*  we should honor the "Csingle" attribute and also write logicals as
        byte rather than long */

    PROTECT(types=allocVector(INTSXP,nvar));
    if (version<=7){
      for(i=0;i<nvar;i++){
        switch(TYPEOF(VECTOR_ELT(df,i))){
          case LGLSXP:
            OutByteBinary(STATA_BYTE,fp);
            break;
           case INTSXP:
            OutByteBinary(STATA_INT,fp);
            break;
          case REALSXP:
            OutByteBinary(STATA_DOUBLE,fp);
            break;
          case STRSXP:
            charlen=0;
            for(j=0;j<nobs;j++){
              k=strlen(CHAR(STRING_ELT(VECTOR_ELT(df,i),j)));
              if (k>charlen)
                charlen=k;
            }
            OutByteBinary((unsigned char)(charlen+STATA_STRINGOFFSET),fp);
            INTEGER(types)[i]=charlen;
            break;
          default:
            error(_("unknown data type"));
            break;
        }
      }
    } else { /* version 8 */
      for(i=0;i<nvar;i++){
        switch(TYPEOF(VECTOR_ELT(df,i))){
          case LGLSXP:
            OutByteBinary(STATA_SE_BYTE,fp);
            break;
          case INTSXP:
            OutByteBinary(STATA_SE_INT,fp);
            break;
          case REALSXP:
            OutByteBinary(STATA_SE_DOUBLE,fp);
            break;
          case STRSXP:
            charlen=0;
            for(j=0;j<nobs;j++){
              k=strlen(CHAR(STRING_ELT(VECTOR_ELT(df,i),j)));
              if (k>charlen)
                charlen=k;
            }
            OutByteBinary((unsigned char)(charlen+STATA_SE_STRINGOFFSET),fp);
            INTEGER(types)[i]=charlen;
            break;
          default:
            error(_("unknown data type"));
            break;
        }
      }
    }
    /** names truncated to 8 (or 32 for v7-8) characters**/

    PROTECT(names=getAttrib(df,R_NamesSymbol));
    for (i=0;i<nvar;i++){
        strncpy(aname,CHAR(STRING_ELT(names,i)),namelength);
        OutStringBinary(nameMangleOut(aname,namelength),fp,namelength);
        OutByteBinary(0,fp);
    }



    /** sortlist -- not relevant **/

    for (i=0;i<2*(nvar+1);i++)
        OutByteBinary(0,fp);

    /** format list: arbitrarily write numbers as %9g format
        but strings need accurate types */
    for (i=0;i<nvar;i++){
        if (TYPEOF(VECTOR_ELT(df,i))==STRSXP){
          /* string types are at most 128 characters
              so we can't get a buffer overflow in sprintf **/
            sprintf(strformat,"%%%ds",INTEGER(types)[i]);
            OutStringBinary(strformat,fp,12);
        } else {
            OutStringBinary(format9g,fp,12);
        }
    }

    /** value labels.  These are stored as the names of label formats,
        which are themselves stored later in the file.
        The label format has the same name as the variable. **/


    for(i=0;i<nvar;i++){
            if (VECTOR_ELT(leveltable,i)==R_NilValue){ /* no label */
                    for(j=0;j<namelength+1;j++)
                            OutByteBinary(0,fp);
            } else {                                   /* label */
                    strncpy(aname,CHAR(STRING_ELT(names,i)),namelength);
                    OutStringBinary(nameMangleOut(aname,namelength),fp,namelength);
                    OutByteBinary(0,fp);
            }
    }


    /** Variable Labels -- full R name of column**/
    /** FIXME: this is now just the same abbreviated name **/

    PROTECT(orig_names=getAttrib(df,install("orig.names")));
    for(i=0;i<nvar;i++) {
        strncpy(datalabel,CHAR(STRING_ELT(orig_names,i)),81);
        datalabel[80]=(char) 0;
        OutStringBinary(datalabel,fp,81);
    }
    UNPROTECT(1);


    /** variable 'characteristics' -- not relevant**/
    OutByteBinary(0,fp);
    OutByteBinary(0,fp);
    OutByteBinary(0,fp);
    if (version>=7) { /*longer in version 7. This is wrong in the manual*/
        OutByteBinary(0,fp);
        OutByteBinary(0,fp);
    }


    /** The Data **/


    for(i=0;i<nobs;i++){
        for(j=0;j<nvar;j++){
            switch (TYPEOF(VECTOR_ELT(df,j))) {
            case LGLSXP:
                OutDataByteBinary(LOGICAL(VECTOR_ELT(df,j))[i],fp);
                break;
            case INTSXP:
                OutIntegerBinary(INTEGER(VECTOR_ELT(df,j))[i],fp,0);
                break;
            case REALSXP:
                OutDoubleBinary(REAL(VECTOR_ELT(df,j))[i],fp,0);
                break;
            case STRSXP:
                k=length(STRING_ELT(VECTOR_ELT(df,j),i));
                OutStringBinary(CHAR(STRING_ELT(VECTOR_ELT(df,j),i)),fp,k);
                for(l=INTEGER(types)[j]-k;l>0;l--)
                    OutByteBinary(0,fp);
                break;
            default:
                error(_("this should not happen."));
                break;
            }
        }
    }

    /** value labels: pp92-94 of 'Programming' manual in v7.0 **/

    for(i=0;i<nvar;i++){
            if (VECTOR_ELT(leveltable,i)==R_NilValue)
                    continue; /* no labels */
            else {
                    theselabels=VECTOR_ELT(leveltable,i);
                    len=4*2*(length(theselabels)+1);
                    txtlen=0;
                    for (j=0;j<length(theselabels);j++){
                            txtlen+=strlen(CHAR(STRING_ELT(theselabels,j)))+1;
                    }
                    len+=txtlen;
                    OutIntegerBinary(len,fp,0); /* length of table */
                    strncpy(aname,CHAR(STRING_ELT(names,i)),namelength);
                    OutStringBinary(nameMangleOut(aname,namelength),fp,namelength);
                    OutByteBinary(0,fp); /* label format name */
                    OutByteBinary(0,fp); OutByteBinary(0,fp); OutByteBinary(0,fp); /*padding*/
                    OutIntegerBinary(length(theselabels),fp,0);
                    OutIntegerBinary(txtlen,fp,0);
                    /* offsets */
                    len=0;
                    for (j=0;j<length(theselabels);j++){
                            OutIntegerBinary(len,fp,0);
                            len+=strlen(CHAR(STRING_ELT(theselabels,j)))+1;
                    }
                    /* values: just 1,2,3,...*/
                    for (j=0;j<length(theselabels);j++){
                            OutIntegerBinary(j+1,fp,0);
                    }
                    /* the actual labels */
                    for(j=0;j<length(theselabels);j++){
                            len=strlen(CHAR(STRING_ELT(theselabels,j)));
                            OutStringBinary(CHAR(STRING_ELT(theselabels,j)),fp,len);
                            OutByteBinary(0,fp);
                            txtlen-=len+1;
                            if (txtlen<0) error(_("this should happen: overrun"));
                    }
                    if (txtlen>0) error(_("this should happen: underrun"));
            }
    }


    UNPROTECT(1); /*names*/

    UNPROTECT(1); /*types*/


}

SEXP do_writeStata(SEXP call)
{
    SEXP fname,  df,leveltable;
    FILE *fp;
    int version;

    if ((sizeof(double)!=8) | (sizeof(int)!=4) | (sizeof(float)!=4))
      error(_("cannot yet read write .dta on this platform"));


    if (!isValidString(fname = CADR(call)))
        error(_("first argument must be a file name\n"));


    fp = fopen(R_ExpandFileName(CHAR(STRING_ELT(fname,0))), "wb");
    if (!fp)
        error(_("unable to open file"));

    df=CADDR(call);
    if (!inherits(df,"data.frame"))
        error(_("data to be saved must be in a data frame"));

    version=INTEGER(coerceVector(CADDDR(call),INTSXP))[0];
    if ((version<6) || (version>8))
        error(_("can only write version 6-8 formats"));
    leveltable=CAD4R(call);

    R_SaveStataData(fp,df,version,leveltable);
    fclose(fp);
    return R_NilValue;
}
