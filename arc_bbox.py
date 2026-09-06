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
def a(x,y):
    if x<0 or y<0 or x>=w or y>=h: return 0
    return out[y*stride+x*bpp+3]

# families: row, xoff (arc inside tile x offset), name
FAMS=[(0,0,"LD/DL"),(2,0,"LU/UL"),(1,20,"UR/RU"),(3,20,"RD/DR")]
for row,xoff,name in FAMS:
    # examine a representative frame column, col=0 for LD/LU, col=0 (x-offset relevant)
    basey=row*75
    # tight bbox over cols 0..14 xoff..xoff+55, full row height 0..75
    minx=10**9;maxx=-1;miny=10**9;maxy=-1
    for fy in range(0,75):
        for fx in range(xoff, xoff+55):
            if a(75*fx//75+ (fx if False else 0) + 0,0)>0: pass
    # correct iteration: within a family the tile x advances by 75 per frame; frame 0 tile starts at 0
    # inspect frame 0 tile: x=0..75, y=row*75..row*75+75 ; arc occupies xoff.. and some yoffset
    tx0=0; ty0=basey
    for yy in range(75):
        for xx in range(75):
            if a(tx0+xx, ty0+yy)>30:
                if xx<minx:minx=xx
                if xx>maxx:maxx=xx
                if yy<miny:miny=yy
                if yy>maxy:maxy=yy
    print(f"{name:8s} frame0 tile: opaque_bbox_rel = x[{minx}..{maxx}] (xoff+0={minx}, +{(maxx-minx)+1}w)  y[{miny}..{maxy}] ({(maxy-miny)+1}h)   abs_y_row_base={basey}")

print()
print("Now inspect where the 55-tall crop should start for each arc (its visual vertical offset).")
for row,xoff,name in FAMS:
    basey=row*75
    # decide: for each frame column, find y of first opaque and last opaque to see consistency over frames
    offs=set()
    for col in [0,4,7,14]:
        tx0=col*75
        ytop=10**9; ybot=-1
        for yy in range(75):
            for xx in range(xoff, xoff+55):
                if a(tx0+xx, basey+yy)>120:
                    if yy<ytop:ytop=yy
                    if yy>ybot:ybot=yy
        offs.add((col,ytop,ybot))
    print(f"{name:8s} across frames (col,ytop,ybot):", sorted(offs))