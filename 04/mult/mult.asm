// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Mult.asm

// Multiplies R0 and R1 and stores the result in R2.
// (R0, R1, R2 refer to RAM[0], RAM[1], and RAM[2], respectively.)

// Put your code here.

	
	@R2
	M=0
	@R3
	M=0
	(Einstieg)
	@0
	D=M
	@R3
	D=M-D
	
	@Ende
	D;JGE
	
	@R1
	D=M
	@R2
	M=D+M
	
	@R3
	M=M+1
	
	@Einstieg
	0;JMP
	(Ende)
	