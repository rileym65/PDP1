	jmp  200
200/	lac  (-377	/ Start count at -377
	dac  _cnt	/ counter variable
	lac  (1000	/ Starting address
	dac  _pntr	/ pointer for writes
	law  0		/ byte to write
	dac  _val	/ store it
lp1,	idx  _val	/ get byte, increment
	dac  i pntr	/ store to destination
	idx  pntr	/ point to next location
	isp  cnt	/ increment count
	jmp  lp1	/ loop until done

	law  0		/ Select tape unit 0
	lio  (1377	/ Final address
	muf		/ Tape unit and final address
	jmp  end	/ jump to end if failed
	lio  (1000	/ Initial address
	mic  1300	/ Start it off
	jmp  end	/ jump to end if failed
lp2,	mes		/ Get status
	and  (40000	/ See if busy
	sza		/ skip if done
	jmp  lp2	/ loop until done
end,	hlt		/ halt

const
variables

	

