import struct, zlib
p="/workspace/WaterPipe-Android/app/src/main/res/drawable-nodpi/allwater.png"
data=open(p,"rb").read(); pos=8; idat=b""; w=h=None
while pos<len(data):
    ln=struct.unpack(">I",data[pos:pos+4])[0]; typ=data[pos+4:pos+8]; chunk=data[pos+8:pos+8+ln]
    if typ==b"IHDR": w,h,bitdepth,ct,cf,fl,inter=struct.unpack(">IIBBBBB",chunk)
    elif typ==b"IDAT": idat+=chunk
    elif typ==b"IEND": break
    pos+=12+ln
raw=zlib.decompress(idat); bpp=4; stride=w*bpp
def paeth(a,b,c):
    pa,pb,pc=b-a,c-b,abs(a+b-2*c)
    if pa<pb:return a if pa<pc else c
    return b if pb<pc else c
out=bytearray(len(raw)); prev=bytearray(stride); rl=stride+1
for y in range(h):
    row=raw[y*rl:(y+1)*rl]; ft=row[0]; line=bytearray(row[1:])
    for x in range(stride):
        a=line[x-bpp] if x>=bpp else 0; b=prev[x]; c=prev[x-bpp] if x>=bpp else 0
        if ft==1:line[x]=(line[x]+a)&0xFF
        elif ft==2:line[x]=(line[x]+b)&0xFF
        elif ft==3:line[x]=(line[x]+((a+b)>>1))&0xFF
        elif ft==4:line[x]=(line[x]+paeth(a,b,c))&0xFF
    out[y*stride:(y+1)*stride]=line; prev=line
def a(x,y):return out[y*stride+x*bpp+3]
def r(x0,x1,y0,y1,tag):
    print("== %s src=(%d,%d)..(%d,%d) =="%(tag,x0,y0,x1,y1))
    for y in range(y0,y1,2):
        s=""
        for x in range(x0,x1,2):
            al=a(x,y)
            s+='#' if al>=200 else('+' if al>=100 else('.' if al>=40 else('-' if al>=10 else ' ')))
        print(s)
    print()
# LD arc f=1 source (0,0,55,55)
r(0,75,0,75,"LD arc (row0)")
# LU (row2) f=1 x=0 (0,150,55,205)
r(0,75,150,225,"LU arc (row2)")
# UR (row1) f=1 x=20 (20,75,75,130)
r(20,95,75,150,"UR arc (row1, x+20)")
# RD (row3) f=1 x=20 (20,75,225,300)
r(20,95,225,300,"RD arc (row3, x+20)")