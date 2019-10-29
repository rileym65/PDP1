	lio  (000221
	msm
	lio  (123456
	lac  (50		/ 10us
del1,	sub  (1			/ 10us
	sza			/ 5us
	jmp  del1		/ 5us
don1,	mwc			/ 5us
	ril  6s			/ 5us   10us
	add  (0			/ 10us  20us
	add  (0			/ 10us  30us
	add  (0			/ 10us  40us
	add  (0			/ 10us  50us
	add  (0			/ 10us  60us
	nop			/ 5 us  65us
ch2,	mwc			/ 5us
	ril  6s			/ 5us   10us
	add  (0			/ 10us  20us
	add  (0			/ 10us  30us
	add  (0			/ 10us  40us
	add  (0			/ 10us  50us
	add  (0			/ 10us  60us
	nop			/ 5 us  65us
ch3,	mwc			/ 5us

/ Delay 255ms, total of 260us after last mwc
/ Then execute mcb to write end of record
	law  14			/ 5 us
	sub  (1			/ 10 us
	sza			/ 5 us
	jmp  .-2		/ 5 us
	add  (0
	law  0
	mcb

/ Stop tape drive
	lio  (0
	msm
	hlt
const




