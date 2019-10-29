	lac	zero
	dac	count
loop	lio	count
	tyo
	idx	count
	lac	count
	and	ff
	sza
	jmp	loop
	hlt
zero	dat	0
count	dat	0
ff	dat	255

