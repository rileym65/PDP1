loop,	cks			/ Check io status
	dio  _temp		/ transfer status
	lac  temp		/ to ac
	and  (040000		/ check key ready flag
	sza			/ skip if key not ready
	jmp  good		/ need to read key
	jmp  loop		/ loop back to look
good,	tyi			/ input key from trypwriter
	tyo			/ output to typewriter
	jmp  loop		/ loop for more
	variables
	constants
	start loop



	





