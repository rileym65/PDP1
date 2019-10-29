	lio  (000201
	msm
	cli
lp1,	szf  i 2
	jmp  lp1
	add  (0		/ 10us
	add  (0		/ 20us
	nop		/ 25us
	mrc
lp2,	szf  i 2
	jmp  lp2
	ril  6s		/ 5us
	add  (0		/ 15us
	add  (0		/ 25us
	mrc
lp3,	szf  i 2
	jmp  lp3
	ril  6s		/ 5us
	add  (0		/ 15us
	add  (0		/ 25 us
	mrc
	dio  _var1
	lio  (0
	msm
	lio  var1
	hlt
variables

const



