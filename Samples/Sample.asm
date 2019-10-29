define	feed  n
	law  i n
	cli
	ppa
	add  (1
	spa
	jmp  .-3
	term

define	exchange
	rcr  9s
	rcr  9s
	term

define  print a
	lio  (a
	tyo
	term

define	print3 a
	lio  (a
	ril  6s
	tyo
	ril  6s
	tyo
	ril  6s
	tyo
	term

go,	feed  100
	print char r
go1,	clf  1
	szf  i 1
	jmp  .-1
	tyi
	dio  _tem
	lac  tem
	add  (ral
	dac  . 2
	law  5252
	hlt
	and  (100
	ior  tem
	exchange
	ppa
	law  char r
	sas  tem
	jmp  go1
go2,	feed 200
	print3 flexo ok
	hlt
	jmp  go

variables
constants
start go


