using FTN95;
using Salford.Fortran;
using System;
using System.Diagnostics;

public sealed class FortranLibrary
{
    private static int[] DebugInfo;
    private static string DebugInfoNames;
    public static float MSAEONE;
    public static float MSAESMALL;
    public static float MSAETOL;
    public static float MSAETWO;
    public static float MSAEZERO;

    public static unsafe void MSAE(int k, int n, int kpn, int imax, int jmax, float x, float xbar, float y, float ybar, float alpha, float b, float sae, int idep, int ifault, float a, float c, int @is, int nb)
    {
        // This item is obfuscated and can not be translated.
        int J;
        int I;
        float H;
        float AA;
        int II;
        float WUN;
        int num1 = imax + -1;
        if (num1 >= 0)
        {
            goto Label_000E;
        }
        int _A_size_1 = -1;
        int num2 = jmax + -1;
        if (num2 >= 0)
        {
            goto Label_0022;
        }
        int num3 = k + -1;
        if (num3 >= 0)
        {
            goto Label_0031;
        }
        int num4 = kpn + -1;
        if (num4 >= 0)
        {
            goto Label_0040;
        }
        int num5 = n + -1;
        if (num5 >= 0)
        {
            goto Label_004F;
        }
        int _X_size_1 = -1;
        int num6 = k + -1;
        if (num6 >= 0)
        {
            goto Label_005E;
        }
        int num7 = k + -1;
        if (num7 >= 0)
        {
            goto Label_006D;
        }
        int num8 = n + -1;
        if (num8 >= 0)
        {
            goto Label_007C;
        }
        int num9 = imax + -1;
        if (num9 >= 0)
        {
            goto Label_008B;
        }
        int num10 = jmax + -1;
        if (num10 >= 0)
        {
            goto Label_009F;
        }
        ifault = 0;
        int KP1 = k + 1;
        int KN = n + k;
        int NP1 = n + 1;
        if (n < k)
        {
            ifault = 1;
        }
        if (imax != NP1)
        {
            ifault = 2;
        }
        if (jmax != KP1)
        {
            ifault = 3;
        }
        if (kpn != KN)
        {
            ifault = 4;
        }
        if (ifault > 0)
        {
            return;
        }
        int LCOL = jmax + -1;
        int LoopUpper1 = jmax;
        for (J = 1; LoopUpper1 >= J; J++)
        {
            (((( _A_size_1) + 1) * (( J) + -1)) * 4)[(int) a] =  MSAEZERO;
        }
        int LoopUpper2 = k;
        for (I = 1; LoopUpper2 >= I; I++)
        {
            ((( I) * 4) + -4)[(int) b] =  MSAEZERO;
            ((( I) * 4) + -4)[(int) c] =  MSAEZERO;
            ((( I) * 4) + -4)[(int) nb] =  I;
        }
        int LoopUpper3 = kpn;
        for (I = KP1; LoopUpper3 >= I; I++)
        {
            ((( I) * 4) + -4)[(int) c] =  MSAETWO;
        }
        int LoopUpper4 = k;
        for (J = 1; LoopUpper4 >= J; J++)
        {
            int LoopUpper5 = n;
            for (I = 1; LoopUpper5 >= I; I++)
            {
                int Temp15 = ((int) ( J)) + -1;
                (((Temp15 * (( _A_size_1) + 1)) + I) * 4)[(int) a] = (int) ((((((( _X_size_1) + 1) * Temp15) + I) * 4) + -4)[(int) x] - ((( J) * 4) + -4)[(int) xbar]);
            }
        }
        int LoopUpper6 = n;
        for (I = 1; LoopUpper6 >= I; I++)
        {
            (( I) * 4)[(int) @is] =  (k + I);
            ((((jmax + -1) * (( _A_size_1) + 1)) + I) * 4)[(int) a] = (int) (((( I) * 4) + -4)[(int) y] - ybar);
        }
    Label_0301:
        H = -MSAETOL;
        int ICAND = 0;
        Logical3 DONE = Logical3.TRUE;
        int LoopUpper7 = imax;
        for (I = 2; LoopUpper7 >= I; I++)
        {
            AA = ((float*) (((((( _A_size_1) + 1) * (jmax + -1)) + I) * 4) + -4))[(int) a];
            if (AA < H)
            {
                DONE = Logical3.FALSE;
                H = AA;
                ICAND = I;
            }
        }
        if (DONE == Logical3.FALSE)
        {
            float RSAVE;
            int JCAND = 0;
            float RATIO = MSAESMALL;
            int LoopUpper8 = LCOL;
            for (J = 1; LoopUpper8 >= J; J++)
            {
                int IONE = 1;
                AA = ((float*) (((((( _A_size_1) + 1) * (( J) + -1)) + ICAND) * 4) + -4))[(int) a];
                if (Math.Abs((double) AA) >= MSAETOL)
                {
                    float RCOST = ((float*) (((( _A_size_1) + 1) * (( J) + -1)) * 4))[(int) a];
                    if (-MSAETOL <= AA)
                    {
                        IONE = -1;
                        if (((( J) * 4) + -4)[(int) nb] >= IntPtr.Zero)
                        {
                            goto Label_041E;
                        }
                        if (-((( J) * 4) + -4)[(int) nb] > k)
                        {
                            RCOST -= MSAETWO;
                        }
                    }
                    float R = RCOST / AA;
                    if (R > RATIO)
                    {
                        JCAND = IONE * J;
                        RATIO = R;
                        RSAVE = RCOST;
                    }
                }
            }
            int IT = ((int*) ((( ICAND) * 4) + -4))[(int) @is];
            if (IT >= 0)
            {
                goto Label_0489;
            }
            II = -IT;
            float CJ = ((float*) ((( II) * 4) + -4))[(int) c];
            if (-CJ >= RATIO)
            {
                ((( ICAND) * 4) + -4)[(int) @is] = -((( ICAND) * 4) + -4)[(int) @is];
                int LoopUpper9 = jmax;
                for (J = 1; LoopUpper9 >= J; J++)
                {
                    int Temp16 = (int) ((( _A_size_1) + 1) * (( J) + -1));
                    (Temp16 * 4)[(int) a] = ((int) ((((Temp16 + ICAND) * 4) + -4)[(int) a] * CJ)) + (Temp16 * 4)[(int) a];
                    int Temp17 = ((int) ((( _A_size_1) + 1) * (( J) + -1))) + ICAND;
                    ((Temp17 * 4) + -4)[(int) a] = -((Temp17 * 4) + -4)[(int) a];
                }
            }
            else
            {
                WUN = MSAEONE;
                if (JCAND <= 0)
                {
                    JCAND = -JCAND;
                    ((( JCAND) * 4) + -4)[(int) nb] = -((( JCAND) * 4) + -4)[(int) nb];
                    WUN = -MSAEONE;
                    (((( _A_size_1) + 1) * (( JCAND) + -1)) * 4)[(int) a] =  RSAVE;
                }
                float PIVOT = (((((( _A_size_1) + 1) * (( JCAND) + -1)) + ICAND) * 4) + -4)[(int) a] * WUN;
                int LoopUpper10 = jmax;
                for (J = 1; LoopUpper10 >= J; J++)
                {
                    int Temp18 = ((int) ((( _A_size_1) + 1) * (( J) + -1))) + ICAND;
                    ((Temp18 * 4) + -4)[(int) a] = (int) (((Temp18 * 4) + -4)[(int) a] / PIVOT);
                }
                int LoopUpper11 = imax;
                for (I = 1; LoopUpper11 >= I; I++)
                {
                    if (ICAND != I)
                    {
                        float AIJ = (((((( _A_size_1) + 1) * (( JCAND) + -1)) + I) * 4) + -4)[(int) a] * WUN;
                        if (AIJ != MSAEZERO)
                        {
                            int LoopUpper12 = jmax;
                            for (J = 1; LoopUpper12 >= J; J++)
                            {
                                int Temp19 = (int) ((( _A_size_1) + 1) * (( J) + -1));
                                int Temp20 = Temp19 + I;
                                ((Temp20 * 4) + -4)[(int) a] = ((int) -((((Temp19 + ICAND) * 4) + -4)[(int) a] * AIJ)) + ((Temp20 * 4) + -4)[(int) a];
                            }
                            (((((( JCAND) + -1) * (( _A_size_1) + 1)) + I) * 4) + -4)[(int) a] =  -(AIJ / PIVOT);
                        }
                    }
                }
                (((((( _A_size_1) + 1) * (( JCAND) + -1)) + ICAND) * 4) + -4)[(int) a] =  (MSAEONE / PIVOT);
                ((( ICAND) * 4) + -4)[(int) @is] = ((( JCAND) * 4) + -4)[(int) nb];
                ((( JCAND) * 4) + -4)[(int) nb] =  IT;
            }
            goto Label_0301;
        }
        alpha = ybar;
        int LoopUpper13 = imax;
        for (I = 2; LoopUpper13 >= I; I++)
        {
            WUN = MSAEONE;
            II = ((int*) ((( I) * 4) + -4))[(int) @is];
            if (II >= 0)
            {
                goto Label_0844;
            }
            if (-II <= k)
            {
                if (II <= 0)
                {
                    II = -II;
                    WUN = -MSAEONE;
                }
                ((( II) * 4) + -4)[(int) b] =  ((((((( _A_size_1) + 1) * (jmax + -1)) + I) * 4) + -4)[(int) a] * WUN);
                alpha = -(((( II) * 4) + -4)[(int) b] * ((( II) * 4) + -4)[(int) xbar]) + alpha;
            }
        }
        float LSAE = (float) -(((( _A_size_1) + 1) * (jmax + -1)) * 4)[(int) a];
        idep = 1;
        int LoopUpper14 = LCOL;
        for (J = 1; LoopUpper14 >= J; J++)
        {
            if (((( J) * 4) + -4)[(int) nb] >= IntPtr.Zero)
            {
                goto Label_094F;
            }
            if (-((( J) * 4) + -4)[(int) nb] <= k)
            {
                idep = 0;
                break;
            }
        }
    }
}

