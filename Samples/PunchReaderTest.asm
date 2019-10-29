100/
	lio  (377		/ All ones
	jsp  pnch10		/ Punch 10 of them
	lio  (0			/ All zeroes
	jsp  pnch10		/ Punch 10 of them
	lio  (377		/ All ones
	jsp  pnch10		/ Punch 10 of them
	lio  (0			/ All zeroes
	jsp  pnch10		/ Punch 10 of them
	lio  (0			/ Prefix of zero
	jsp  p376		/ Punch 0,0-0,376
	lio  (377		/ Prefix of all ones
	jsp  p376		/ Punch 377,0-377,376
/ Now punch 00-77 in binary mode
	lac  (0			/ Start at zero
	dac  count		/ Save it
blp,	lio  count		/ Get current
	ppb			/ punch in binary
	lac  count		/ get current
	sad  (770000		/ Jump if done
	jmp  done		/ jump if done
	add  (10000		/ add increment
	dac  count		/ store it
	jmp  blp		/ otherwise loop back
done,	hlt

/ Punch 10 of whatever is in IO
pnch10,	dap  p10r		/ setup return
	lac  (-12		/ Count of 10
	dac  _count		/ Track for count
p10lp,	ppa			/ punch it
	isp  count		/ increment count
	jmp  p10lp		/ loop until done
p10r,	jmp  0			/ return to caller

/ Punch 00-376 with prefix passed in IO
p376,	dap  p376r		/ setup return
	dio  _prefix		/ save prefix
	lac  (0			/ start next group at 0
	dac  count		/ Store in counter
lp1,	lio  prefix		/ start with prefix
	ppa			/ punch it
	lio  count		/ get current
	ppa			/ punch it
	idx  count		/ increment by 1
	sas  (377		/ Skip if terminator
	jmp  lp1		/ Loop until done
p376r,	jmp  0			/ Return to caller

variables
constants

