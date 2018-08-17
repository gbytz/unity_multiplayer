#import <Foundation/Foundation.h>
#import <SceneKit/SceneKit.h>
#import <JidoMaps/JidoMaps-Swift.h>
#import <ARKit/ARKit.h>
@class JidoSession;
@class ARSession;

typedef NS_ENUM(NSInteger, Mode)
{
    ModeMapping = 0,
    ModeLocalization = 1,
};

@interface JidoSessionWrapper : NSObject

+ (instancetype)sharedInstanceWithARSession:(ARSession *)session mapMode:(Mode)mode mapId: (NSString*) mapId userId:(NSString*) userId developerKey: (NSString*) developerKey screenHeight: (float)screenHeight screenWidth: (float)screenWidth;

+ (instancetype)sharedInstance;

+ (void)setUnityCallbackGameObject:(NSString*)objectName;
+ (void)setAssetLoadedCallbackFunction:(NSString*)functionName;
+ (void)setStatusUpdatedCallbackFunction:(NSString*)functionName;
+ (void)setStorePlacementCallbackFunction:(NSString*)functionName;
+ (void)setProgressCallbackFunction:(NSString*)functionName;
+ (void)setObjectDetectedCallbackFunction:(NSString*)functionName;

- (instancetype)initWithARSession:(ARSession *)session mapMode:(Mode)mode mapId: (NSString*) mapId userId:(NSString*) userId developerKey: (NSString*) developerKey screenHeight: (float)screenHeight screenWidth: (float)screenWidth;

- (void)uploadAssets:(NSArray*)array;

- (void)update:(ARFrame*) frame;

- (void)planeDetected:(ARAnchor*) anchor;

- (void)planeRemoved:(ARAnchor*) anchor;

- (void)planeUpdated:(ARAnchor*) anchor;

- (const char*)multiplayerSync:(float) x1 y1:(float)y1 z1:(float)z1 x2:(float)x2 y2:(float)y2 z2:(float)z2 x3:(float)x3 y3:(float)y3 z3:(float)z3 x4:(float)x4 y4:(float)y4 z4:(float)z4
                            qx:(float)qx qy:(float)qy qz:(float)qz qw:(float)qw isQuaternionInitialized:(BOOL)isQuaternionInitialized;

- (void)dispose;

@end
