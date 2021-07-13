      SUBROUTINE MSAE(K, N, KPN, IMAX, JMAX, X, XBAR, Y, YBAR, ALPHA, B, SAE, IDEP, IFAULT, A, C, IS, NB)
!
!     ALGORITHM AS 108  APPL. STATIST. (1977), VOL. 26, NO.1
!
!     Computes the minimum sum of absolute errors (MSAE) estimates of
!     unknown parameters ALPHA, B(1), B(2), ... , B(K) in the linear
!     regression equation:
!
!     Y(I) = ALPHA + B(1)X(1) + B(2)X(2) + ... + B(K)X(K)
!
!     The regression line passes through the point
!     ( XBAR(1), XBAR(2), ... , XBAR(K), YBAR ) , 
!     where these are the mean values of these variables,
!     though the values of XBAR() and YBAR are input by the user, and
!     could be any point through which the line is to be forced.
!     Users should read the criticism of this algorithm on page 378 of
!     volume 27 of Applied Statistics (1978).  
!     One way to use this algorithm would be to fix the XBAR's, say
!     equal to the means, but to vary the value of YBAR until the minimum
!     value of LSAE is found.
!
!     An alternative source of Fortran algorithms for this task is:
!        Gonin, R. and Money, A.H. (1989) Nonlinear Lp-norm estimation,
!        Dekker: New York.
!
!     N.B. The user should check the output values of both IFAULT & IDEP.
!
      REAL A(IMAX, JMAX), B(K), C(KPN), X(N, K), XBAR(K), Y(N)
      INTEGER IS(IMAX), NB(JMAX)
      LOGICAL DONE
      REAL SMALL, TOL, ZERO, ONE, TWO
      REAL LSAE
      DATA SMALL/-1.E+10/, TOL/1.E-08/, ZERO/0.0/, ONE/1.0/, TWO/2.0/
!
!     Check for parameter consistency
!
      IFAULT = 0
      KP1 = K + 1
      KN = K + N
      NP1 = N + 1
      IF (K .GT. N) IFAULT = 1
      IF (IMAX .NE. NP1) IFAULT = 2
      IF (JMAX .NE. KP1) IFAULT = 3
      IF (KPN .NE. KN) IFAULT = 4
      IF (IFAULT .GT. 0) RETURN
!
!     Set up the initial tableau.
!
      LCOL = JMAX - 1
      DO 1 J = 1, JMAX
    1 A(1, J) = ZERO
      DO 10 I = 1, K
	B(I) = ZERO
	C(I) = ZERO
	NB(I) = I
   10 CONTINUE
      DO 20 I = KP1, KPN
   20 C(I) = TWO
      DO 30 J = 1, K
	DO 30 I = 1, N
	  A(I+1, J) = X(I, J) - XBAR(J)
   30 CONTINUE
      DO 40 I = 1, N
	IS(I+1) = I + K
	A(I+1, JMAX) = Y(I) - YBAR
   40 CONTINUE
!
!     Determine the variable to leave the basis.
!
   50 H = -TOL
      ICAND = 0
      DONE = .TRUE.
      DO 80 I = 2, IMAX
	AA = A(I, JMAX)
	IF (AA .GE. H) GO TO 80
	DONE = .FALSE.
	H = AA
	ICAND = I
   80 CONTINUE
      IF (DONE) GO TO 200
!
!     Determine the variable to enter the basis.
!
      JCAND = 0
      RATIO = SMALL
      DO 110 J = 1, LCOL
	IONE = 1
	AA = A(ICAND, J)
	IF (ABS(AA) .LT. TOL) GO TO 110
	RCOST = A(1, J)
	IF (AA .LT. -TOL) GO TO 90
	IONE = -1
	IF (ABS(NB(J)) .GT. K) RCOST = RCOST - TWO
   90   R = RCOST / AA
	IF (R .LE. RATIO) GO TO 110
	JCAND = J * IONE
	RATIO = R
	RSAVE = RCOST
  110 CONTINUE
!
!     Determine if an ordinary simplex pivot is unnecessary.
!
      IT = IS(ICAND)
      II = ABS(IT)
      CJ = C(II)
      IF (RATIO .GT. -CJ) GO TO 140
      IS(ICAND) = -IS(ICAND)
      DO 130 J = 1, JMAX
	A(1, J) = A(1, J) + CJ * A(ICAND, J)
	A(ICAND, J) = -A(ICAND, J)
  130 CONTINUE
      GO TO 50
!
!     Perform ordinary simplex pivot.
!
  140 WUN = ONE
      IF (JCAND .GT. 0) GO TO 160
      JCAND = -JCAND
      NB(JCAND) = -NB(JCAND)
      WUN = -ONE
      A(1, JCAND) = RSAVE
  160 PIVOT = A(ICAND, JCAND) * WUN
      DO 170 J = 1, JMAX
  170 A(ICAND, J) = A(ICAND, J) / PIVOT
      DO 190 I = 1, IMAX
	IF (I .EQ. ICAND) GO TO 190
	AIJ = A(I, JCAND) * WUN
	IF (AIJ .EQ. ZERO) GO TO 190
	DO 180 J = 1, JMAX
	  A(I, J) = A(I, J) - A(ICAND, J) * AIJ
  180   CONTINUE
	A(I, JCAND) = -AIJ / PIVOT
  190 CONTINUE
      A(ICAND, JCAND) = ONE / PIVOT
      IS(ICAND) = NB(JCAND)
      NB(JCAND) = IT
      GO TO 50
!
!     Compute ALPHA and the vector B containing the slopes.
!
  200 ALPHA = YBAR
      DO 220 I = 2, IMAX
	WUN = ONE
	II = IS(I)
	IF (ABS(II) .GT. K) GO TO 220
	IF (II .GT. 0) GO TO 210
	II = -II
	WUN = -ONE
  210   B(II) = WUN * A(I, JMAX)
	ALPHA = ALPHA - XBAR(II) * B(II)
  220 CONTINUE
      LSAE = -A(1, JMAX)
!
!     Inspect the final solution for dependencies amongst the predictor
!     variables.
!
      IDEP = 1
      DO 300 J = 1, LCOL
	IF (ABS(NB(J)) .GT. K) GO TO 300
	IDEP = 0
	RETURN
  300 CONTINUE
!
      RETURN
      END

