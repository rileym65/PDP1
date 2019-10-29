cbs = 720056
asc = 720051
	jmp strt
30/
int6,	0
	0
	0
	jmp type2
strt,	cbs
	esm
	asc 600
	lio (msg		/ get address of message
	jsp type		/ call type subroutine
loop,	jmp loop

/ Type function, address is in IO
type,	dio _tpos		/ Store address
	dap tret		/ setup return
tloop,	cks			/ get status bits
	dio _tmp
	lac tmp
	and (100000		/ check if ok to tyo
	sza i			/ loop until can tyo
	jmp tloop		/ loop until ready
	lio i tpos		/ get character to type
	tyo-i+4000
	idx tpos		/ point to next position
tret,	jmp 0			/ return to caller

type2,	iot i			/ clear io sync
	lio i tpos		/ next character
	lac i tpos		/ get char for test
	sza			/ skip if no more
	jmp typ2a		/ jump to send next char
type2d, lac int6		/ recover ac
	lio int6+2		/ recover io
	jmp i int6+1		/ return from break
typ2a,	tyo-i+4000		/ send character
	idx tpos		/ point to next character
	jmp type2d		/ return from break

msg,	char rt
	char re
	char rs
	char rt
	0

	variables
	constants





