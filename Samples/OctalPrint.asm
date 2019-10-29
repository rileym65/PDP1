/ OctalPrint
poc=jda opt
tes,	lat		/ get test word
	poc		/ call octal print
	lio  (77	/ carriage return
	tyo
	hlt
	jmp  tes
100/
opt,	0		/ subroutine start
	dap  opx
	law  i 6
	dac  _occ	/ occ is a variable, count
opc,	lac  opt
	ral  7
	dac  opt
	and  (7		/ get first digit
	sza  i		/ test for digit 0
	law  char r0	/ get concise code for 0
	rcr  777
	rcr  777	/ put code in io
	tyo
	isp  occ	/ more characters
	jmp  opc	/ yes
opx,	jmp  .		/ no, exit
	variables
	constants
	start tes




