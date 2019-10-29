	lac  arg
	jda  decout	
	hlt

/  Routine to print out fractional number
/  AC holds number to print
/  Call with 'jda decout'
decout, 0			/ space for arg
	dap  decret		/ save return address
	lac  decout		/ get argument
	lio  dot		/ get dot
	tyo			/ print it
declp,	sar  1s			/ multiply by 10
	dac  decout
	sar  2s
	add  decout
	dac  decout		/ save answer
	cli			/ Clear io register
	rcl  5s			/ Move to io
	dio  tmp
	lac  tmp
	sza i			/ Skip if not zero
	lio  zero		/ Need 0 character
	tyo			/ output it
	lac  decout		/ recover current
	and  (17777		/ Clear sign bit
	sal  4s			/ Move to new bits
	sza			/ Skip if no more bits
	jmp  declp		/ Loop for remaining
decret,	jmp  0
tmp,	0
zero,	20
dot,	73

arg,	frac  456789

constants




