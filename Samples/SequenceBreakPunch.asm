cbs=720056
asc=720051
brk40=40
	jmp  strt	/ Jump to program start
brk40/
	0		/ break vector 10
	0
	0
	jmp  pnch

200/
strt,	cbs		/ clear break system
	esm		/ enable break mode
	asc+1000	/ activate channel 10
	lac  (-10	/ get punch count
	dac  cnt	/ and store in counter
	lac  (1		/ first number to punch
	dac  chr	/ store it
	lio  chr	/ character to punch
	ppa-i		/ punch first character
lp1,	lac  cnt	/ get current count
	sza		/ skip if last number punched
	jmp  lp1	/ loop forever
	lio  (13	/ stop code
	lsm		/ turn off break mode
	ppa
	hlt

pnch,	lac  cnt	/ get current count
	add  (1		/ add 1
	dac  cnt	/ store it back
	sza i		/ skip it not zero
	jmp  iret	/ return from break if zero
	lac  chr	/ get char to punch
	add  (1		/ increment it
	dac  chr	/ store it back
	lio  chr	/ get character to punch
	ppa-i		/ punch it
iret,	lac  brk40	/ recover ac
	lio  brk40+2	/ recover io
	jmp i brk40+1	/ return from break

cnt,	0
chr,	0

const





