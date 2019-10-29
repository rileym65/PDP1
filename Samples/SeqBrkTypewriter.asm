asc=iot 51
cbs=iot 56
	jmp  main
24/
int5,	0
	0
	0
	jmp  rdkey
main,	esm		/ enable sequence break mode
	cbs		/ clear sequence break mode
	asc  500	/ enable channel 5
loop,	jmp  loop	/ do nothing now
rdkey,	tyi		/ get key from typewriter
	tyo		/ type it back out
	lac  int5	/ recover ac
	lio  int5+2	/ recover io
	jmp  i int5+1	/ exit interrupt routine
	hlt



