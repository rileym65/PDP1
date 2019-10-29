	jmp  200
200/	law  0		/ Tape unit 0
	lio  (1377	/ Ending address
	muf  		/ Set unit and final address
	hlt		/ Halt on error
	lio  (1000	/ Starting address
	mic  1700	/ Read odd
	hlt		/ Halt on error
lp1,	mes		/ Get status
	and  (4000	/ Mask all but busy bit
	sza		/ Skip if no longer busy
	jmp  lp1	/ Loop until done
	hlt		/ Halt

constants


	

