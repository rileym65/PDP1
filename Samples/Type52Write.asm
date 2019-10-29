	jmp  200
200/	
	lio  (1000	/ First block at 1000
	jsp  finc	/ fill it
	lio  (2000	/ Second block at 2000
	law  1		/ Fill with 1s
	jda  fill	/ perform the fill
	lio  (3000	/ Third block at 3000
	law  2		/ Fill with 2s
	jda  fill	/ perform the fill

	lac  (1000
	dac  wrti
	lac  (1400
	dac  wrtf
	law  0
	jda  wrt
	sza
	jmp  err

	lac  (2000
	dac  wrti
	lac  (2400
	dac  wrtf
	law  0
	jda  wrt
	sza
	jmp  err

	lac  (3000
	dac  wrti
	lac  (3400
	dac  wrtf
	law  0
	jda  wrt
	sza
	jmp  err

err,	hlt

wrti,	0		/ Initial address
wrtf,	0		/ Final address
wrt,	0		/ space for tape unit
	dap  wret	/ Setup return
	lac  wrt	/ Get unit number
	lio  wrtf	/ Get final address
	muf		/ Setup unit and final address
	jmp  werr	/ Jump if error
	lio  wrti	/ Get initial address
	mic  1300	/ Perform write odd parity
	jmp  werr	/ Jump if error
wlp,	mes		/ get status
	and  (40000	/ See if busy
	sza		/ Skip if done
	jmp  wlp	/ Loop until done
	law  0		/ Indicate no error
wret,	jmp  0		/ Return to caller
werr,	law  1		/ Indicate error
	jmp  wret	/ and return

/ ***** Fill a memory block with 001 - 100
finc,	dap  frt2	/ Setup retun
	dio  fadr	/ Store address
	lac  (-400	/ 256 bytes to write
	dac  fcnt	/ write to count
	law  		/ First value
	dac  fill	/ Place here
filp,	idx  fill	/ increment and get byte
	dac  i fadr	/ store it
	idx  fadr	/ increment address
	isp  fcnt	/ increment count
	jmp  filp	/ loop until done
frt2,	jmp  0		/ Return to caller

/ ***** Fill a memory block with value in ac
fcnt,	0		/ space for count
fadr,	0		/ space for address
fill,	0		/ space to store value
	dap  fret	/ store return address
	dio  fadr	/ save address
	lac  (-400	/ 256 bytes to write
	dac  fcnt	/ store into count
fllp,	lac  fill	/ get fill character
	dac  i fadr	/ store into dest block
	idx  fadr	/ increment the address
	isp  fcnt	/ increment count
	jmp  fllp	/ loop back if not done
fret,	jmp  0		/ return to caller
const
variables

	


