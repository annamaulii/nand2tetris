// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Fill.asm

// Runs an infinite loop that listens to the keyboard input.
// When a key is pressed (any key), the program blackens the screen,
// i.e. writes "black" in every pixel;
// the screen should remain fully black as long as the key is pressed. 
// When no key is pressed, the program clears the screen, i.e. writes
// "white" in every pixel;
// the screen should remain fully clear as long as no key is pressed.

// Put your code here.
	
(ANFANG)
	@KBD
	D=M
	@WEISS
	D;JEQ
(SCHWARZ)
	
	@R1
	M=0
(SCHLEIFE)
	@R1
	D=M
	@8192
	D=D-A
	@ENDE
	D;JGE
		
	@R1
	D=M
	@SCREEN
	D=D+A
	A=D
	M=-1
	
	@R1
	M=M+1
	@SCHLEIFE
	0;JMP
	
	@ENDE
	0;JMP
	


(WEISS)

	@R1
	M=0
(SCHLEIFEWEISS)
	@R1
	D=M
	@8192
	D=D-A
	@ENDE
	D;JGE
		
	@R1
	D=M
	@SCREEN
	D=D+A
	A=D
	M=0
	
	@R1
	M=M+1
	@SCHLEIFEWEISS
	0;JMP
	
	@ENDE
	0;JMP
	
(ENDE)
	
	@ANFANG
	0;JMP
	
	