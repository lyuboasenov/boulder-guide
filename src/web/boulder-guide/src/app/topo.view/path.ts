export interface Point {
   x: number;
   y: number;
}

export interface Command {
   l: string;
   points: Point[];
   s: string;
}

interface ControlPoint {
   a: number,
   b: number
}

export function calculatePath(points: Point[]): Command[] {
   let items: Array<Command> = [];

   if (points.length == 1) {
      items.push(moveTo(points[0]));
   } else if (points.length == 2) {
      items.push(moveTo(points[0]));
      items.push(lineTo(points[1]));
   } else if (points.length > 2) {
      items.push(moveTo(points[0]));
      let v : number[] = [points.length];
      for (let i = 0; i < points.length; i++) {
         v[i] = points[i].x;
      }

      // get cubic bezier coefficients
      let xCo = getCubicBezierCoefficients(v);

      for (let i = 0; i < points.length; i++) {
         v[i] = points[i].y;
      }

      // get cubic bezier coefficients
      let yCo = getCubicBezierCoefficients(v);

      for (let i = 0; i < points.length - 1; i++) {
         items.push(curveTo({ x: xCo[i].a, y: yCo[i].a }, { x: xCo[i].b, y: yCo[i].b }, { x: points[i + 1].x, y: points[i + 1].y }));
      }
   }

   return items;
}

function getCubicBezierCoefficients(v : number[]) : ControlPoint[] {
   var N = v.length - 1;
   var N2 = N << 1;
   var i = 0;
   var j = 0;
   var r11 : number, r12 : number, r15 : number;               // r13 & r14 always 0.
   var r22 : number, r23 : number, r25 : number;               // r21 & r24 always 0 for all except last equation, where r21 is -1.
   var r31 : number, r32 : number, r33 : number, r34 : number, r35 : number;
   var Rows : number[][] = [];
   for (let p = 0; p < N2; p++) {
      Rows.push([0, 0, 0]);
   }
   var a : number;

   var result : ControlPoint [] = [];
   for (var p = 0; p < N; p++) {
      result.push({ a: 0, b: 0});
   }

   r11 = 2;        // eq 3
   r12 = -1;
   r15 = v[j++];

   r22 = 1;        // eq 1
   r23 = 1;
   r25 = 2 * v[j++];

   r31 = 1;        // eq 2
   r32 = -2;
   r33 = 2;
   r34 = -1;
   r35 = 0;

   while (true) {
      a = 1 / r11;
      r11 = 1;
      r12 *= a;
      r15 *= a;

      // r21 is always 0. No need to eliminate column.
      // r22 is always 1. No need to scale row.

      // r31 is always 1 at this point.
      r31 -= r11;
      r32 -= r12;
      r35 -= r15;

      if (r32 != 0) {
         r33 -= r32 * r23;
         r35 -= r32 * r25;
         r32 = 0;
      }

      // r33 is always 0.

      // r11 always 1.
      Rows[i][0] = r12;
      Rows[i][1] = 0;
      Rows[i][2] = r15;
      i++;

      // r21, r24 always 0.
      Rows[i][0] = r22;
      Rows[i][1] = r23;
      Rows[i][2] = r25;
      i++;

      if (i >= N2 - 2)
         break;

      r11 = r33;
      r12 = r34;
      r15 = r35;

      r22 = 1;        // eq 1
      r23 = 1;
      r25 = 2 * v[j++];

      r31 = 1;        // eq 2
      r32 = -2;
      r33 = 2;
      r34 = -1;
      r35 = 0;
   }

   r11 = r33;
   r12 = r34;
   r15 = r35;

   //r21 = -1;		// eq 4
   r22 = 2;
   r23 = 0;
   r25 = v[j++];

   a = 1 / r11;
   r11 = 1;
   r12 *= a;
   r15 *= a;

   //r21 += r11;
   r22 += r12;
   r25 += r15;

   r25 /= r22;
   r22 = 1;

   // r11 always 1.
   Rows[i][0] = r12;
   Rows[i][1] = 0;
   Rows[i][2] = r15;
   i++;

   // r21 and r24 always 0.
   Rows[i][0] = r22;
   Rows[i][1] = r23;
   Rows[i][2] = r25;
   i++;

   // Then eliminate back up:

   j--;
   while (i > 0) {
      i--;
      if (i < N2 - 1) {
         a = Rows[i][1];
         if (a != 0) {
            Rows[i][1] = 0;
            Rows[i][2] -= a * Rows[i + 1][2];
         }
      }

      result[--j].b = Rows[i][2];

      i--;
      a = Rows[i][0];
      if (a != 0) {
         Rows[i][0] = 0;
         Rows[i][2] -= a * Rows[i + 1][2];
      }

      result[j].a = Rows[i][2];
   }

   return result;
}

function moveTo(p: Point) : Command {
   return getCommand('M', [p]);
}

function lineTo(p: Point) : Command {
   return getCommand('L', [p]);
}

function curveTo(p1: Point, p2: Point, p: Point) : Command {
   return getCommand('C', [p1, p2, p]);
}

function getCommand(letter: string, points: Point[]) : Command {
   let result: Command = {
      l: letter,
      points: points,
      s: ''
   };

   return result;
}
