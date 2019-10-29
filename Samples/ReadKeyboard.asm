loop,	cks		/ Check io status
	dio  _temp	/ transfer status
	lac  temp	/ to ac
	and  keyin	/ check key ready flag
	sza		/ skip if key not ready
	jmp  good	/ need to read key
	jmp  loop	/ loop back to look
good,	tyi		/ input key from trypwriter
	hlt		/ and then halt
keyin,	040000		/ flag for key ready from typewriter
	variables
	start loop

	


