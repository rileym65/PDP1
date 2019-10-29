	lac  (arg1
	jda  tflex
	hlt
arg1,	text 'This is a test'
	0
tflex,	0
	dap  rflex
tflxlp,	lac i tflex		/ Get next word
	sza i			/ Skip if zero
	jmp  rflex		/ Exit routine
	cli
	rcl  6s
	tyo
	cli
	rcl  6s
	tyo
	cli
	rcl  6s
	tyo
	lac  tflex		/ get address
	add  (1			/ Increment it
	dac  tflex		/ Put it back
	jmp  tflxlp		/ Loop back
rflex,	jmp  0

constants




