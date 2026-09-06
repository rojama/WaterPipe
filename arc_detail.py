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
def show(tag,x0,y0,x1,y1,step=1):
    print("==",tag,"  src=(%d,%d)..(%d,%d) step=%d =="%(x0,y0,x1,y1,step))
    for y in range(y0,y1,step):
        s=("%3d "%((y-y0)))+"".join(
            '#' if a(x,y)>=200 else('+' if a(x,y)>=100 else('.' if a(x,y)>=40 else('-' if a(x,y)>=10 else ' ')))
            for x in range(x0,x1,step))
        print(s)
    print()
# LD full arc: row0, col14, frame=15 -> x=1050..1125
show("LD frame15 (row0 col14)",1050,0,1125,75,step=1)
# LD frame1: row0 col0
show("LD frame1 (row0 col0)",0,0,75,75,step=1)
# RD: code stores RD full arc at col0 (revFrame=15=>frame1... wait). RD frame1 uses revFrame=15 col14.
# RD frame15 -> revFrame=1 -> col0. Show col0 and col14.
show("RD col0 (row3)",0,225,75,300,step=1)
show("RD col14 (row3)",1050,225,1125,300,step=1)