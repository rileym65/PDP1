[c]	PDP1 Emulator
[c]	by
[c]	Mike Riley
[=]
[h2]	Control Console:
[=]
[h1]	Switches
[tb-1]
  	| Power          | - | Computer is on when switch is to the left. |

	| Single Step    | - | Computer will run in single step mode when this switch is
                              to the left. |

	| Single Inst.   | - | Computer will run in single instruction mode when this
                              switch is to the left. |

	| Sense Switches | - | The state of these switches can be sensed from within 
                              programs.  They do not serve any purpose on their own. |

	| Extend         | - | The switch when up will cause the computer to start in the
                              extended memory mode when the Start switch or Read In 
                              switches are depressed.  If this switch is down then 
                              normal memory mode will be entered upon Start or Read In. |

	| Extension      | - | These four switches are copied into the extension address
                              when the computer is in Start or Read In mode.  This copy
                              occurs whether or not the Extend switch is on. |

	| Address        | - | These switches allow the user to select a memory address
                              for other operations. |

	| Test Word      | - | These switches set the state of the test word register
                              which can be read programatically. |

	| Start          | - | This momentary switch when activated will start the computer
                              running.  The address/extnension switches will be used to
                              determine at what address the computer will start fetching
                              instructions from.  If this switch is lifted (up) then
                              computer will be started in the Sequence Break mode, when
                              depressed (down) then the computer starts with the 
                              Sequence Break mode disabled. |

	| Stop           | - | This momentary switch when depressed will stop the computer
                              from automatic execution.  The stop will occur after the
                              current memory cycle has completed. |

	| Continue       | - | This momentary switch when depressed will continue 
                              automatic program execution.  No registers are altered by
                              this process so the computer will continue running from
                              where it was stopped. |

	| Examine        | - | This momentary switch when depressed will read the memory
                              value addressed by the Extension/Address switches and
                              display it on the Accumulator and Memory Buffer lights. |

	| Deposit        | - | This momentary switch when depressed will take the value
                              specified by the Test Word switches and write it into 
                              the memory address specified by the Extension/Address
                              switches. |
  
	| Read In        | - | This momentary switch when depressed will start the computer
                              in Read In mode.  This mode reads punched tapes that where
                              created for this mode.  This is the primary way to load
                              a program into memory and begin its execution when no other
                              program is present in memory. |

	| Reader         | - | This switch normally advances the reader paper tape.  This
                              is not needed or supported on this emulator. |

	| Tape Feed      | - | This switch normally advances the paper tape punch.  This is
                              not needed or supported on this emulator. |
[te]
[=]
[h1]	Lamps:
[tb-1]
	| Program Counter | - | These lamps show current state of the program counter.  The
                                  Extension lamps show the state of the current extended 
                                  address register. |

	| Memory Address  | - | These lamps show the last memory address read/written. |

	| Memory Buffer   | - | These lamps show the last value read/written from memory. |

	| Accumulator     | - | These lamps show the current value in the accumulator. |

	| In-Out          | - | These lamps show the current value in the IO register. |

	| Run             | - | This lamp is lit when the computer is running. |

	| Cycle           | - | This lamp lights when at least one cycle of an instruction
	                          has executed and there is at least one more cycle for the
	                          current instruction to be completed. |

	| Defer           | - | This lamp lights whenever the computer is executing a
	                          deferred memory cycle. |

	| H.S. Cycle      | - | This lamp is lit when a High Speed channel request is 
	                          accessing memory. |

	| Brk Ctr. 1/2    | - | These two lamps indicate the special sequence that occurs
	                          during a sequence break.  In the first cycle only Brk Ctr. 1
	                          will be lit, on the second cycle only Brk Ctr. 2 will be 
	                          lit and on the third cycle both will be lit. |

	| Over Flow       | - | This lamp will light anytime the overflow flip flop is set. |

	| Read In         | - | This lamp is lit when the computer is operating in Read In
	                          mode. |

	| Seq. Break      | - | This lamp is lit when the sequence break system is enabled. |

	| Extend          | - | This lamp is lit when the Extend Memory mode is active. |

	| I-O Halt        | - | This lamp is lit when the computer is in the IO wait mode
	                          waiting for an IO operation to complete. |

	| I-O Com'ds      | - | |

	| I-O-Sync        | - | This lamp is lit when an IO device completes and an IO
	                          Wait mode has not been entered. |

	| Power           | - | This lamp is lit when the power is on. |

	| Single Step     | - | This lamp is lit when the computer is operating in the
	                          single step mode. |

	| Single Inst.    | - | This lamp is lit when the computer is operating in the 
	                          single instruction mode. |

	| Sense Switches  | - | This set of lamps indicated which of the sense switches is
	                          on. |

	| Program Flags   | - | These lamps show the status of the program flags. |

	| Instruction     | - | These lamps show the instruction being executed. |
[te]
[=]
[=]
[h2]	Memory:
	  The main memory of the PDP-1 consists of 4096 18-bit words.  There is also
	an extended memory that was available on the PDP-1 which allowed for the 
	addition of 15 more 4096 word pages.  This emulator supports all 16 pages of 
	memory, giving 65536 18-bit words.
[=]
[=]
[h2]	Instruction Format:

[-]	|    Opcode    | I|             Address Y             |
[-]	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
[-]	| 0| 1| 2| 3| 4| 5| 6| 7| 8| 9|10|11|12|13|14|15|16|17|
[-]	|        |        |        |        |        |        |
[=]
[=]
[h2]	Indirect Addressing:
	  The PDP-1 supported multi-level indirect addressing for all instructions that
	referenced a memory argument.  If the indirection bit (Bit 5) of the instruction
	was set then the address contained in the Y field of the instruction acted as
	a pointer into memory where the address of the argument could be found.
[=]
	  If the secondary address also has bit 5 set then the indirection would take
	another step.  Indirection would continue until an address is read that does not
	have bit 5 set, this then would be the actual address for the argument.
[=]
	  Here is an example of how indirection works:
[=]
[i4]
[tb4]
	| [0000]  |  lac i 0002 |
	| [0001]  |  hlt        |
	| [0002]  |  010005     |
	|  ...    |             |
	| [0005]  |  000010     |
	|  ...    |             |
	| [0010]  |  123456     |
[te]
[i-4]
[=]
  Here is what happens when the 'lac i 0002' instruction is executed.  Since 
i is specified bit 5 will be set indicating that the argument is not at 0002
but the address stored at 0002 points to the argument.  When the address at
0002 is read (010005) it also has the I bit set so again the argument is not
at 0005 but the address at 0005 points to the argument.  The number stored
at 0005 (000010) does not have the I bit set so the actual argument is located
at memory cell 0010 and therefore 123456 will be loaded into the AC.
[=]
[=]
[h2]	Instructions:
[=]
[h1]	Arithmetic Instructions
[tb2]
	| #####  |  ###### | ## | |
	| add y  |  40yyyy | 10 | AC = AC + (Y)                                 |
	| div y  |  56yyyy | -- | AC,IO = AC:IO / (Y), skip next if no overflow |
	| idx y  |  44yyyy | 10 | (Y) = (Y) + 1, AC = new (Y)                   |
	| isp y  |  46yyyy | 10 | (Y) = (Y) + 1, AC = new (Y), Skip if (Y) >= 0 |
	| mul y  |  54yyyy | -- | AC:IO = AC * (Y)                              |
	| sub y  |  42yyyy | 10 | AC = AC - (Y)                                 |
[te]
[=]
[h1]	Logical Instructions
[tb2]
	| #####  |  ###### | ## | |
	| and y  |  02yyyy | 10 | AC = AC and (Y) |
	| ior y  |  04yyyy | 10 | AC = AC or (Y)  |
	| xor y  |  06yyyy | 10 | AC = AC xor (Y) |
[te]
[=]
[h1]	Basic instructions
[tb2]
	| #####  |  ###### | ## | |
	| cal    |  16xxxx | 10 | (100) = AC, AC= OXE EEE PPP PPP PPP PPP. PC=101 |
	| dac y  |  24yyyy | 10 | (Y) = AC                                        |
	| dap y  |  26yyyy | 10 | (Y) = AC  Only address part is set              |
	| dio y  |  32yyyy | 10 | (Y) = IO                                        |
	| dip y  |  30yyyy | 10 | (Y) = AC  Only instruction part is set          |
	| dzm y  |  34yyyy | 10 | (Y) = 0                                         |
	| iot y  |  720000 |  5 | IOT group, will act as nop                      |
	| jda y  |  17yyyy | 10 | (Y) = AC, AC = OXE EEE PPP PPP PPP PPP. PC=Y+1  |
	| jmp y  |  60yyyy |  5 | PC = Y                                          |
	| jsp y  |  62yyyy |  5 | AC = OXE EEE PPP PPP PPP PPP. PC = Y            |
	| lac y  |  20yyyy | 10 | AC = (Y)                                        |
	| law n  |  70nnnn |  5 | AC = n                                          |
	| lio y  |  22yyyy | 10 | IO = (Y)                                        |
	| opr    |  760000 |  5 | Operate group, Will act as nop                  |
	| sad y  |  50yyyy | 10 | Skip if AC <> (Y)                               |
	| sas y  |  52yyyy | 10 | Skip if AX == (Y)                               |
	| sft    |  660000 |  5 | Shift group, will act as nop                    |
	| skp    |  640000 |  5 | Skip group, will act as nop                     |
	| xct y  |  10yyyy |  5+|  Execute instruction found at (Y)               |
[te]
[=]
[h1]	Shift group
[tb2]
	| #####  |  ###### | ## | |
	| ral n  |  661nnn |  5 | Roate AC left for each 1 bit in n     |
	| rar n  |  671nnn |  5 | Roate AC right for each 1 bit in n    |
	| rcl n  |  663nnn |  5 | Roate AC:IO left for each 1 bit in n  |
	| rcr n  |  673nnn |  5 | Roate AC:IO right for each 1 bit in n |
	| ril n  |  662nnn |  5 | Roate IO left for each 1 bit in n     |
	| rir n  |  672nnn |  5 | Roate IO right for each 1 bit in n    |
	| sal n  |  665nnn |  5 | Shift AC left for each 1 bit in in    |
	| sar n  |  675nnn |  5 | Shift AC right for each 1 bit in n    |
	| scl n  |  667nnn |  5 | Shift AC:IO left for each 1 bit in in |
	| scr n  |  677nnn |  5 | Shift AC:IO right for each 1 bit in n |
	| sil n  |  666nnn |  5 | Shift IO left for each 1 bit in in    |
	| sir n  |  676nnn |  5 | Shift IO right for each 1 bit in n    |
[te]
[=]
[h1]	Skip group
[tb2]
	| #####  |  ###### | ## | |
	| sma    |  640400 |  5 | Skip if AC < 0                      |
	| spa    |  640200 |  5 | Skip if AC >= 0                     |
	| spi    |  642000 |  5 | Skip if IO >= 0                     |
	| sza    |  640100 |  5 | Skip if AC = 0                      |
	| szf    |  64000f |  5 | Skip on zero flag                   |
	| szo    |  641000 |  5 | Skip if overflow = 0                |
	| szs n  |  6400n0 |  5 | Skip if sense switch is zero,
	                          If n=7 then all switches must be 0  |
[te]
[=]
[h1]	Operate group
[tb2]
	| #####  |  ###### | ## | |
	| cla    |  760200 |  5 | AC = 0                                 |
	| clf n  |  76000n |  5 | Clear program flag.  nnn = 1 through 7 |
	| cli    |  764000 |  5 | IO = 0                                 |
	| cma    |  761000 |  5 | AC = not AC                            |
	| hlt    |  760400 |  5 | Halt the computer                      |
	| lap    |  760100 |  5 | AC = AC or OXE EEE PPP PPP PPP PPP     |
	| lat    |  762000 |  5 | AC = AC OR test word switches          |
	| nop    |  760000 |  5 | nop                                    |
	| stf n  |  76001n |  5 | Set program flag.  nnn = 1 through 7   |
[te]
[=]
[h1]	IO
[tb2]
	| #####  |  ###### | ## | |
	| cks    |  720033 |    | Set IO to status flags |
[te]
[=]
[h1]	Extend memory control
[tb2]
	| #####  |  ###### | ## | |
	| eem    |  724074 |    | Enter extend mode |
	| lem    |  720074 |    | Leave extend mode |
[te]
[=]
[h1]	Paper tape reader
[tb2]
	| #####  |  ###### | ## | |
	| rpa    |  720001 |    | Read paper tape in alpha mode  |
	| rpb    |  720002 |    | Read paper tape in binary mode |
	| rrb    |  720030 |    | Read reader buffer             |
[te]
[=]
[h1]	Paper tape punch
[tb2]
	| #####  |  ###### | ## | |
	| ppa    |  720005 |    | Punch tape in alpha mode  |
	| ppb    |  720006 |    | Punch tape in binary mode |
[te]
[=]
[h1]	Typewriter terminal
[tb2]
	| #####  |  ###### | ## | |
	| tyo    |  720003 |    | Type out |
	| tyi    |  720004 |    | Type in  |
[te]
[=]
[h1]	Card punch type 40-1
[tb2]
	| #####  |  ###### | ## | |
	| lag    |  720044 |    | Loads next group |
	| pac    |  720043 |    | Punch a card     |
[te]
[=]
[h1]	Card Reader type 421
[tb2]
	| #####  |  ###### | ## | |
	| rac    |  720041 |    | Read alpha from card   |
	| rbc    |  720042 |    | Read binary from card  |
	| rcc    |  720032 |    | Read column register   |
[te]
[=]
[h1]	Sequence break system type 120
[tb2]
	| #####  |  ###### | ## | |
	| esm    |  720055 |    | Enter sequence break mode          |
	| lsm    |  720054 |    | Leave sequence break mode          |
	| cbs    |  720056 |    | Clear sequence break system        |
	| dsc    |  72kn50 |    | Deactivate sequence break channel  |
	| asc    |  72kn51 |    | Activate sequence break channel    |
	| isb    |  72kn52 |    | Initiate sequence break            |
	| cac    |  720053 |    | Clear all channels                 |
[te]
[=]
[h1]	High speed data control type 131
[tb2]
	| #####  |  ###### | ## | |
	| swc    |  72x046 |    | Set word counter                |
	| sia    |  720346 |    | Set location counter            |
	| sdf    |  720146 |    | Stop data flow                  |
	| rlc    |  720366 |    | Read location counter           |
	| shr    |  720446 |    | Set high speed channel request  |
[te]
[=]
[h1]	Precision CRT display type 30
[tb2]
	| #####  |  ###### | ## | |
	| dpy    |  720007 |    | Display one point  |
[te]
[=]
[h1]	Symbol generator type 33
[tb2]
	| #####  |  ###### | ## | |
	| gpl    |  722027 |    | Generator plot left        |
	| gpr    |  720027 |    | Generator plot right       |
	| glf    |  722026 |    | Load format                |
	| gsp    |  720026 |    | Space                      |
	| sdb    |  722007 |    | Load buffer, no intensity  |
[te]
[=]
[h1]	Ultra-precision crt display type 31
[tb2]
	| #####  |  ###### | ## | |
	| dpp    |  720407 |    | Display one point  |
[te]
[=]
[h1]	Programmed magnetic tape control type 51
[tb2]
	| #####  |  ###### | ## | |
	| msm    |  720073 |    | Select mode        |
	| mcs    |  720034 |    | Check status       |
	| mcb    |  720070 |    | Clear buffer       |
	| mwc    |  720071 |    | Write a character  |
	| mrc    |  720072 |    | Read character     |
[te]
[=]
[h1]	Automatic magnetic tape control type 52
[tb2]
	| #####  |  ###### | ## | |
	| muf    |  72ue76 |    | Tape Unit and FinalT                   |
	| mic    |  72ue75 |    | Initial and command                    |
	| mrf    |  72u067 |    | Reset final                            |
	| mri    |  72ug66 |    | Reset initial                          |
	| mes    |  72u035 |    | Examine states                         |
	| mel    |  72u036 |    | Examine location                       |
	| inr    |  72ur67 |    | Initiate a high speed channel request  |
	| ccr    |  72s067 |    | Clear command register                 |
[te]
[=]
[h1]	Automatic magnetic tape control type 510
[tb2]
	| #####  |  ###### | ## | |
	| sfc    |  720072 |    | Skip if tape control free      |
	| rsr    |  720172 |    | Read state register            |
	| crf    |  720272 |    | Clear end-of-record flip-flop  |
	| cpm    |  720472 |    | Clear proceed mode             |
	| dur    |  72xx70 |    | Load density, unit, rewind     |
	| mtf    |  72xx71 |    | Load tape function register    |
	| cgo    |  720073 |    | Clear Go                       |
[te]
[=]
[h1]	Automatic line printer type 64
[tb2]
	| #####  |  ###### | ## | |
	| clrbuf |  722045 |    | Clear buffer         |
	| lpb    |  720045 |    | Load printer buffer  |
	| pas    |  721x45 |    | Print and space      |
[te]
[=]

[h2]	Assembler
	  This emulator includes a built in assembler to assist in writing programs
	for the PDP-1.  The original PDP-1 had two assemblers available for it: MACRO
	and FRAP.  This assembler is more or less compatable with programs written for
	MACRO.  I chose MACRO as the model since I considered it to be a bit more
	powerful than FRAP.  At the end of this secion I will give some pointers on
	how to recognize when an assembly source was meant for FRAP as well as some
	tips on how to convert FRAP source to assemble using this assembler.  This 
	assembler is not 100% work-alike of MACRO but will assemble most MACRO source
	with little modification.  The differences between this assembler and MACRO
	will be detailed later.
[=]
	  Terms used while describing the assembler:
[tb1-]
	| number       | - | A string of characters consisting of only characters 0
	                     through 9.                                                   |
  
	| symbol       | - | A string of characters consisting of characters 0 through
	                     9 or a through z.  A symbol must contain at least one letter
	                     to be considered a symbol.                                   |

	| term         | - | Any number or symbol.                                        |

	| operator     | - | Any single character: + - & \| ( _ . <space>                 |

	| terminator   | - | Any single character: / , ) = <tab> <cr>                     |

	| expression   | - | 1 or more terms seperated by operators and ending with a
	                     terminator.                                                  |

	| definite     | - | An expression that contained no undefined symbols.           |

	| indefinite   | - | An expression that contained at least 1 undefined symbol.    |
[te]
[=]
	  The assembler works by scanning the source code evaluating expressions.  Any
	time a terminator is found the prior expression is considered complete and 
	depending on what terminator was found different processes could occur. 
	Expressions cannot span lines since the <cr> character is a terminator.  It is
	possible however to have a single line of source code contain several 
	expressions all separated by <tab>s.
[=]
	  Each expression is started with having a value of zero.  As additional
	terms and operators are scanned the expression value will be changed.  Once
	a terminator is reached the expression value will be used and then cleared.
[=]
	  During expression evaluation the following chracters will perform the
	following functions:
[=]
[tb1-]
	  | +       | - | The next term will be added to the expression value.                |

	  | <space> | - | This performs the same thing as a + does if no other operators are
	                  specified.                                                          |

	  | -       | - | The next term will be subtracted from the expression
	                  value.                                                              |

	  | &       | - | The next term will be logically and'd with the expression value.    |

	  | \|      | - | The next term will be logically or'd with the expression value.     |

	  | .       | - | This is actually a term and stands for the current assembly
	                  address.  It will be combined with the expression value based 
	                  upon the operator before it.                                        |

	  | *       | - | This is actually a term and means the same thing as I, set the 
	                  indirect bit.                                                       |

	  | _       | - | This indicates that the symbol immediately following this character
	                  should be added to the list of variables.  There can be no
	                  other characters between the _ and the symbol.  If the symbol
	                  is already defined then its value will be combined with the
	                  expression value based upon the preceeding operator.  If the symbol
	                  is not yet defined then the current expression will be marked as
	                  indefinite.                                                         |

	  | (       | - | This indicates that the following expression is a constant that
	                  should be entered into the constant table.  If the expression 
	                  after the ( is definite then the value will be added to the 
	                  expression value that this constant is a part of.  If the 
	                  expression following ( is indefinite then the expression before the
	                  ( will also be indefinite                                           |
[te]
[=]
	  All terms encountered during expression evaluation will be combined with
	the current expression value based upon the operator immediately preceeding
	the term.  If any term is an undefined symbol then the result of the 
	expression will be indefinite.  If an expression evaluates as indefinite 
	during pass 2 of the assembly then an error will result.
[=]
	  It should be noted that all additions or subtractions from the expression
	value are done in 1s compliment.
[=]
	  When a terminator is found during expression evaluation then that expression
	is done and will be operated on by the terminator then cleared and then the
	assembler will evaluate the next expression.  The terminators perfrom the
	following functions:
[=]
[tb1-]
	  | <tab>   | - | If the preceeding expression had at least 1 term then the 
	            expression value will be written out as a memory value at the
	            current assembly address and then the assembly address will be
	            incremented.                                                               |

	  | <cr>    | - | This works exactly like <tab>.                                       |

	  | /       | - | The preceeding expression will be used as the new assembly address.
	            The current expression value must be definite.  If the current
	            value is indefinite then an error will result.                             |

	  | ,       | - | This terminator normally ends the definition of an address label.
	            The preceeding expression must consist of just a single symbol and
	            it must be indefinite.  If the preceeding expression consisted of
	            more than 1 term, or a single term that was a number instead of
	            a symbol or a symbol that is defined then an error will result.
	            The , terminator is also used as an expression separator for
	            REPEAT, DEFINE and macro invokations.                                      |

	  | =       | - | This terminator will allow the symbol preceeding it to be defined
	            to the expression following the =.  The expression before the =
	            must consist of a single term and it must be a symbol.  It does
	            not matter if the symbol is definite or not, it will be set to
	            the value of the expression following the =.  If the expression
	            following the = is indefinite then the symbol being assigned will
	            be considered indefinite.  If during pass 2 the following
	            expression is indefinite then an error will result.  The = can
	            change the value of any symbol in the symbol table, including the
	            built in ones.                                                             |

	  | )       | - | This terminator completes the expression before it and enteres the
	            value into the constants table.  If the expression between ( and )
	            is indefinite then the constant will be considered indefinite.
	            If the constant is indefinite during pass 2 then an error will
	            result.  The ) does not terminate any expression that was being
	            evaluated before the ( and that expression is still considered to
	            be being evaluated.  The constant will be combined with the 
	            expression value based upon the operator preceeding the (
	            character.  If the constant was indefinite then the expression
	            containing the constant will also be indefinite.                           |
[te]
[=]
	  An expression is considered to be in evaluation if at least 1 term was 
	scanned.  Some pseudo-ops can only be performed if there is no current 
	expression while others can be used within expressions.  The section on 
	pseudo-ops will specify if a given pseudo-op can be used in expressions or not.
[=]
[h1]	Assembler Pseudo-Ops
[tb1]
	| START expr       | - | Specify start address, ends assembly pass  |
	| OCTAL            | - | Set radix to base-8                        |
	| DECIMAL          | - | Set radix to base-10                       |
	| FRAC number      | - | Converts the decimal number into fraction  |
	| expr/            | - | Set current address to expression          |
	| label,           | - | Set symbal to current address              |
	| label=expr       | - | Set label equal to expression              |
	| CHAR Rchar       | - | Get character code in right 6 bits         |
	| CHAR Mchar       | - | Get character code in middle 6 bits        |
	| CHAR Lchar       | - | Get character code in left 6 bits          |
	| FLEX chrchrchr   | - | Encode next 3 characters into single word  |
	| TEXT chrstrchr   | - | Encode str to series of FLEX words         |
	| .                | - | Current address                            |
	| _symb            | - | Defines symb as a variable                 |
	| VARIABLES        | - | Assemble variables                         |
	| (expr)           | - | Define a constant                          |
	| CONSTANTS        | - | Assemble constants                         |
	| DEFINE name [arg1,...argn]  | - | Define a macro                  |
	| TERMINATE        | - | End macro definition                       |
	| EXPUNGE          | - | Delete all symbols from the symbol table   |
[te]
[=]
[h1]	Differences between this assembler and PDP1 MACRO
[lb]
[li]	This assembler adds & and | to the valid operators when evaluating 
	expressions.  The original assembler did not have these.
[li]	The CONSTANTS command may only be used once per assembly whereas with the 
	original assembler you could use it multiple times.
[li]	A variable is designated by preceeding a symbol with the underscore (_)
	character.  In the original assembler an overstrike could be placed over
	any of the characters to designate the symbol as a variable.
[li]	This assembler will never punch the source program on an output tape, as
	such the NOINPUT pseudo-op has no meaning.
[li]	This assembler requires the DEFINE to be on the same line as the macro
	header.  The original assembler allowed these to be on separate lines.
[li]	This assember adds the FRAC pseudo-op for easily producing fractional
	values.
[li]	This assembler accepts the * character in the same role as I to indicate
	indirection.
[le]
[=]
[h1]	Sequence Break Vectors
[i4]
[tb]
	| Channel  |  Start Address  |
	|   0      |       00        |
	|   1      |       04        |
	|   2      |       10        |
	|   3      |       14        |
	|   4      |       20        |
	|   5      |       24        |
	|   6      |       30        |
	|   7      |       34        |
	|  10      |       40        |
	|  11      |       44        |
	|  12      |       50        |
	|  13      |       54        |
	|  14      |       60        |
	|  15      |       64        |
	|  16      |       70        |
	|  17      |       74        |
[te]
[i-4]
[=]
[h1]	Sequence Break Block
[i4]
[tb]
	| +0 | AC gets stored here         |
	| +1 | PC gets stored here         |
	| +2 | IO gets stored here         |
	| +3 | Address of service routine  |
[te]
[i-4]
[=]
[h1]	Return from Sequence Break
[i4]
[tb]
	| lac | vec+0    |
	| lio | vec+2    |
	| jmp | i vec+1  |
[te]
[i-4]
[=]

[h1]	Assembler Predefined Symbols
[i4]
[tb]	
	| Symbol | Value  |
	| 1S     | 000001 |
	| 2S     | 000003 |
	| 3S     | 000007 |
	| 4S     | 000017 |
	| 5S     | 000037 |
	| 6S     | 000077 |
	| 7S     | 000177 |
	| 8S     | 000377 |
	| 9S     | 000777 |
	| ADD    | 400000 |
	| AND    | 020000 |
	| CAL    | 160000 |
	| CDF    | 720074 |
	| CFD    | 720074 |
	| CKS    | 720033 |
	| CLA    | 760200 |
	| CLC    | 761200 |
	| CLF    | 760000 |
	| CLI    | 764000 |
	| CLO    | 651600 |
	| CMA    | 761000 |
	| DAC    | 240000 |
	| DAP    | 260000 |
	| DIO    | 320000 |
	| DIP    | 300000 |
	| DIV    | 560000 |
	| DPY    | 730007 |
	| DZM    | 340000 |
	| ESM    | 720055 |
	| HLT    | 760400 |
	| I      | 010000 |
	| IDX    | 440000 |
	| IOR    | 040000 |
	| IOT    | 720000 |
	| ISP    | 460000 |
	| JDA    | 170000 |
	| JFD    | 120000 |
	| JMP    | 600000 |
	| JSP    | 620000 |
	| LAC    | 200000 |
	| LAP    | 760300 |
	| LAT    | 762200 |
	| LAW    | 700000 |
	| LIO    | 220000 |
	| LSM    | 720054 |
	| MUL    | 540000 |
	| NOP    | 760000 |
	| OPR    | 760000 |
	| PPA    | 730005 |
	| PPB    | 730006 |
	| RAL    | 661000 |
	| RAR    | 671000 |
	| RCL    | 663000 |
	| RCR    | 673000 |
	| RIL    | 662000 |
	| RIR    | 672000 |
	| RPA    | 730001 |
	| RPB    | 730002 |
	| RRB    | 720030 |
	| SAD    | 500000 |
	| SAL    | 665000 |
	| SAR    | 675000 |
	| SAS    | 520000 |
	| SCR    | 677000 |
	| SIL    | 666000 |
	| SIR    | 676000 |
	| SKP    | 640000 |
	| SMA    | 640400 |
	| SPA    | 640200 |
	| SPI    | 642000 |
	| SPQ    | 650500 |
	| STF    | 760010 |
	| SUB    | 420000 |
	| SZA    | 640100 |
	| SZF    | 640000 |
	| SZM    | 640500 |
	| SZO    | 641000 |
	| SZS    | 640000 |
	| TYI    | 720004 |
	| TYO    | 730003 |
	| XCT    | 100000 |
	| XOR    | 060000 |
	| XX     | 760400 |
	| ASC    | 730051 |
	| CAC    | 730053 |
	| CBS    | 730056 |
	| CCR    | 730067 |
	| CGO    | 730073 |
	| CLRBUF | 732045 |
	| CPM    | 730472 |
	| CRF    | 730272 |
	| DPP    | 730407 |
	| DSC    | 730050 |
	| DUR    | 730070 |
	| EEM    | 734074 |
	| GLF    | 732026 |
	| GPL    | 732027 |
	| GPR    | 730027 |
	| GSP    | 730026 |
	| INR    | 730067 |
	| ISB    | 730052 |
	| LAG    | 730044 |
	| LEM    | 730074 |
	| LPB    | 730045 |
	| MBS    | 720375 |
	| MCB    | 730070 |
	| MCE    | 721475 |
	| MCO    | 721575 |
	| MCS    | 730034 |
	| MES    | 720035 |
	| MEL    | 730036 |
	| MFE    | 721075 |
	| MFO    | 721175 |
	| MIC    | 720075 |
	| MRC    | 730072 |
	| MRE    | 721675 |
	| MRF    | 730067 |
	| MRO    | 721775 |
	| MRW    | 720175 |
	| MRI    | 730066 |
	| MSM    | 730073 |
	| MST    | 720075 |
	| MTF    | 730071 |
	| MUF    | 730076 |
	| MWC    | 730071 |
	| MWE    | 721275 |
	| MWO    | 721375 |
	| PAC    | 730043 |
	| PAS    | 731045 |
	| RAC    | 730041 |
	| RBC    | 730042 |
	| RCC    | 730032 |
	| RLC    | 730366 |
	| RSR    | 730172 |
	| SCL    | 667000 |
	| SDB    | 732007 |
	| SDF    | 730146 |
	| SFC    | 730072 |
	| SFT    | 660000 |
	| SHR    | 730446 |
	| SIA    | 730346 |
	| SWC    | 730046 |
	| SB0    | 000000 |
	| SB1    | 000004 |
	| SB2    | 000010 |
	| SB3    | 000014 |
	| SB4    | 000020 |
	| SB5    | 000024 |
	| SB6    | 000030 |
	| SB7    | 000034 |
	| SB10   | 000040 |
	| SB11   | 000044 |
	| SB12   | 000050 |
	| SB13   | 000054 |
	| SB14   | 000060 |
	| SB15   | 000064 |
	| SB16   | 000070 |
	| SB17   | 000074 |
	| C0     | 000000 |
	| C1     | 000100 |
	| C2     | 000200 |
	| C3     | 000300 |
	| C4     | 000400 |
	| C5     | 000500 |
	| C6     | 000600 |
	| C7     | 000700 |
	| C10    | 001000 |
	| C11    | 001100 |
	| C12    | 001200 |
	| C13    | 001300 |
	| C14    | 001400 |
	| C15    | 001500 |
	| C16    | 001600 |
	| C17    | 001700 |
	| S1     | 000001 |
	| S2     | 000003 |
	| S3     | 000007 |
	| S4     | 000017 |
	| S5     | 000037 |
	| S6     | 000077 |
	| S7     | 000177 |
	| S8     | 000377 |
	| S9     | 000777 |
	| MTCU0  | 000000 |
	| MTCU1  | 002000 |
	| MTCU2  | 004000 |
[te]
[i-4]
[=]
[h1]	Magtapes:
[i4]
[-]	2400 feet long (28,800 inches)
[-]	200 chars (6 bits plus parity) /inch
[-]	5,760,000 chars per tape
[-]	read/write speed 75 inches/sec, 15,000 chars/sec. 65us between
[-]	Rewind speed 500 inches/sec, 100,000 chars/sec. 10us between
[-]	End of record gap .75 inches (150 chars)
[-]	Start time 5ms
[-]	Stop time 3ms
[=]
[i-4]
[h2]	Type 51 Magnetic Tape Control
[h1]	Magnetic Tape Select Mode (5us)
[=]
	msm 720073
[=]
	Transfers IO to command buffer.  Bits:
[=]
[i4]
[tb]
	| Bit | If Zero        | If One      |
	| --- | -------------- | ----------- |
	| 10  | Do not operate | Operate     |
	| 11  | Do not Rewind  | Rewind      |
	| 12  | Forward        | Reverse     |
	| 13  | Read           | Write       |
	| 14  | Odd Parity     | Even Parity |
	| 15  | 75 inches/sec. | Not Used    |
[te]
[=]
[tb]
	| Bit 16 | Bit 17 | Transport    |
	| ------ | ------ | ------------ |
	| 0      | 0      | No Transport |
	| 0      | 1      | Transport 1  |
	| 1      | 0      | Transport 2  |
	| 1      | 1      | Transport 3  |
[te]
[i-4]
[=]
[h1]	Magnetic Tape Check Status (5us)
[=]
	mcs 720034
[=]
	Transfers io status to bits 0 through 7 of IO
[=]
[i4]
[tb]
	| Bit | If One               | If Zero                                   |
	| --- | -------------------- | ---------------------------------------   |
	| 0   | Not Ready            | Ready                                     |
	| 1   | Not Rewinding        | Rewinding                                 |
	| 2   | Not In File Protect  | In File Protect (write protected)         |
	| 3   | Not at Loat Point    | At Load Point (~10 feet from end)         |
	| 4   | Not Full Tape Supply | Full Tape Supply (~100 ft on takeup reel) |
	| 5   | Not Low Tape Supply  | Low Tape Supply (~100 ft on source reel)  |
	| 6   | Not at End Point     | At End Point (~14 ft from trailing end)   |
	| 7   | In Manual Mode       | In Automatic Mode                         |
[te]
[i-4]
[=]
[h1]	Magnetic Tape Write Chracter (5us)
[=]
	mwc 720071
[=]
	Writes from bits 0-5 of IO to tape.
[=]
	First write should not occur before 3 milliseconds after starting tape.
[-]	Successive writes need to be 65us apart.
[-]	After last write, before end of record wait 260us.
[=]
[h2]	Type 52 Magnetic Tape Control
[h1]	Magnetic Tape Unit and Final (5us)
[=]
	muf 72ue76
[=]
 	TapeUnitRegister = AC:15..17
[-]	FinalAddressRegister = IO:2..17
[-]	StopContinueRegister = Inst:10..11
[-]	Bits 5,8,9 of instruction should be 0
[=]
	Unit-End table (ue)
[=]
[i4]
[tb]
	| Function                   | Op  | MTCU0   | MTCU 1  | MTCU 2  |
	| -------------------------- | --- | ------- | ------- | ------- |
	| Normal completion and stop | mnp | 000 000 | 010 000 | 100 000 |
	| Early completion and stop  | mep | 000 001 | 010 001 | 100 001 |
	| Early completion and cont  | mec | 000 011 | 010 011 | 100 011 |
[te]
[i-4]
[=]
[h1]	Magnetic Tape Initial and Command (5us)
[=]
	mic 72uc75
[=]
	CurrentAddressRegister = IO:2..17
[-]	CommandRegister = inst:8..11
[-]	Bit 5 should be 0
[=]
	Command table (uc)
[=]
[i4]
[tb]
	| Function                 | Op  | MTCU 0  | MTCU 1  | MTCU 2  |
	| ------------------------ | --- | ------- | ------- | ------- |
	| Stop                     | mst | 000 000 | 010 000 | 100 000 |
	| Rewind                   | mrw | 000 001 | 010 001 | 100 001 | 
	| Backspace                | mbs | 000 011 | 010 011 | 100 011 |
	| Forward space even       | mfe | 001 000 | 011 000 | 101 000 |
	| Forward space odd        | mfo | 001 001 | 011 001 | 101 001 |
	| Write even parity        | mwe | 001 010 | 011 010 | 101 010 |
	| Write odd parity         | mwo | 001 011 | 011 011 | 101 011 |
	| Read/compare even parity | mce | 001 100 | 011 100 | 101 100 |
	| Read/compare odd parity  | mco | 001 101 | 011 101 | 101 101 |
	| Read even parity         | mre | 001 110 | 011 110 | 101 110 |
	| Read odd parity          | mro | 001 111 | 011 111 | 101 111 |
[te]
[i-4]
[=]
[h1]	Magnetic Tape Reset Final (5us)
[=]
	mrf 72u067
[=]
	FinalAddressRegister = IO:2..17
[=]
	bits 5,8..11 should be 0
[=]
[h1]	Magnetic Tape Reset Initial (5us)
[=]
	mri 72ug66
[=]
[-]	CurrentAddressRegister = IO:2..17
[-]	CommandRegister:2 = inst:10
[=]
[h1]	Magnetic Tape Examine States (5us)  
[=]
	mes 72u035
[=]
	bits 5,8..11 should be 0
[=]
[i4]
[tb]
	| IO Register | State of MTCU and Transport                     |
	| ----------- | ----------------------------------------------- |
	| Bit 0=1     | One or more of the conditions marked * exist    |
	| Bit 1=1     | Parity error                                    |
	| Bit 2=1     | Missing character error                         |
	| Bit 3=1     | MTCU busy, if zero then record complete         |
	| Bit 4=1     | Read-compare error                              |
	| Bit 5=1     | Illegal tape command                            |
	| Bit 6=1     | High speed channel request late                 |
	| Bit 7=1     | CA=FA, Addresses equal                          |
	| Bit 8=1     | Selected tape transport ready                   |
	| Bit 10=1    | Selected TT is rewinding                        |
	| Bit 11=1    | Selected TT is in file protect                  |
	| Bit 12=1    | Selected TT is at load point                    |
	| Bit 13=1    | Selected TT has full tape supply                |
	| Bit 14=1    | Selected TT has low tape supply                 |
	| Bit 15=1    | Selected TT is beyond end point                 |
	| Bit 16=1    | Early completion and stop has been selected     |
	| Bit 17=1    | Early completion and continue has been selected |
[te]
[i-4]
[=]
[h1]	Magnetic Tape Examine Location (5us)
[=]
	mel 72u036
[=]
	IO = CurrentAddressRegister
[=]
	bits 5,8..11 should be 0
[=]
[h2]	Others instructions
[=]
[i4]
[tb]
	| dio y | 01 101i yyyy yyyy yyyy | (Y) = IO          |
	| jmp y | 11 0000 yyyy yyyy yyyy | PC = Y            |
	| eem   | 11 1010 1000 0011 1100 | Enter extend mode |
	| lem   | 11 1010 0000 0011 1100 | Leave extend mode |
[te]
[i-4]
[=]
[h2]	IO instructions:
[=]
	111 01a b
[=]
[i4]
[tb]
	| a | b | Function                                       |
	| - | - | ---------------------------------------------- |
	| 1 | 0 | enter io-halt until io is complete             |
	| 0 | 1 | proceed but require io-wait instruction 730000 |
	| 1 | 1 | No completion pulse will be recieved           |
	| 0 | 0 | No completion pulse will be recieved           |
[te]
[i-4]
[=]
[h2]	Status word
[=]
[-]	abc def ghi jkl mno pqr
[-]	001 010 000 000 000 000
[=]
[i4]
[tb]
	| bit | When 0                                 | When 1                         |
	|  a  | start of dpy                           | light pulse strikes pen        |
	|  b  | no data to read from paper tape reader | tape reader ready to read      |
	|  c  | typewriter not ready                   | typewriter ready to receive    |
	|  d  | no keys ready from typewriter          | key ready to read              |
	|  e  | paper tape punch busy                  | punch ready for next character |
[te]
[i-4]
[=]
[h2]	Character Codes
[=]
[i4]
[tb]
	| FIO-DEC | CONCISE | CHARACTER |  |
	|   CODE  |   CODE  | LOWER     | UPPER |
	| ------- | ------- | -----     | ----- |
	|    00   |    00   | Tape Feed | |
	|     -   |   100   | Delete    | |
	|    00   |   200   | Space     | |
	|    01   |    01   | 1         | " |
	|    02   |    02   | 2         | ' |
	|    03   |   203   | 3         | ~ |
	|    04   |    04   | 4         | (implies) |
	|    05   |   205   | 5         | (or) |
	|    06   |   206   | 6         | (and) |
	|    07   |    07   | 7         | < |
	|    10   |    10   | 8         | > |
	|    11   |   211   | 9         | (up arrow) |
	|     -   |    13   | Stop Code | |
	|    20   |    20   | 0         | (right arrow) |
	|    21   |   221   | /         | ? |
	|    22   |   222   | s         | S |
	|    23   |    23   | t         | T |
	|    24   |   224   | u         | U |
	|    25   |    25   | v         | V |
	|    26   |    26   | w         | W |
	|    27   |   227   | x         | X |
	|    30   |   230   | y         | Y |
	|    31   |    31   | z         | Z |
	|    33   |   233   | ,         | = |
	|    34   |     -   | black *   | |
	|    35   |     -   | red *     | |
	|    36   |   236   | tab       | |
	|    40   |    40   | _         | (non-spacing middle dot) |
	|    41   |   241   | j         | J |
	|    42   |   242   | k         | K |
	|    43   |    43   | l         | L |
	|    44   |   244   | m         | M |
	|    45   |    45   | n         | N |
	|    46   |    46   | o         | O |
	|    47   |   247   | p         | P |
	|    50   |   250   | q         | Q |
	|    51   |    51   | r         | R |
	|    54   |    54   | -         | + |
	|    55   |   255   | )         | ] |
	|    56   |   256   | [pipe]    | (non-spacing overstrike) |
	|    57   |    57   | (         | [ |
	|    61   |    61   | a         | A |
	|    62   |    62   | b         | B |
	|    63   |   263   | c         | C |
	|    64   |    64   | d         | D |
	|    65   |   265   | e         | E |
	|    66   |   266   | f         | F |
	|    67   |    67   | g         | G |
	|    70   |    70   | h         | H |
	|    71   |   271   | i         | I |
	|    72   |   272   | Lower Case | |
	|    73   |    73   | .         | (multiply) |
	|    74   |   274   | Upper Case | |
	|    75   |    75   | Backspace | |
	|    77   |   277   | Carriage Return | |
[te]
[=]
	* Used on Type-Out only, not on keyboard
[i-4]
[=]

