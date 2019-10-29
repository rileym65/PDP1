	lac  (arg1
	jda  tflex
	hlt
arg1,	text 'This is a test'
	0
tflex,	0
	dap  rflex+1
tflxlp,	lac i tflex		/ Get next word
	sza i			/ Skip if zero
	jmp  rflex		/ Exit routine
	cli
	rcl  6s
	lpb
	cli
	rcl  6s
	lpb
	cli
	rcl  6s
	lpb
	lac  tflex		/ get address
	add  (1			/ Increment it
	dac  tflex		/ Put it back
	jmp  tflxlp		/ Loop back
rflex,	pas
	jmp  0

constants





